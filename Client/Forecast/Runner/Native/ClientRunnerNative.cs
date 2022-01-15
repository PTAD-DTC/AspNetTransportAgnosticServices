using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Runner.Base;
using ForecastClient.Runner.Base;
using ForecastService.Interface;
using ForecastService.Interface.Models.DTO;
using Service.Interface.Base;

namespace ForecastClient.Runner.Native
{
    public class ClientRunnerNative : ForecastClientRunnerBase<IForecastService>
    {
        private readonly Func<(Uri serviceUri, string? apiVersion), CancellationToken, IForecastService> _serviceFactory;

        public ClientRunnerNative(
            IClientRunnerUserInterface userInterface, bool canSelectServiceUrl,
            Func<(Uri serviceUri, string? apiVersion), CancellationToken, IForecastService> serviceFactory) : base(userInterface)
        {
            CanSelectServiceUrl = canSelectServiceUrl;
            _serviceFactory = serviceFactory;
        }

        protected override bool CanSelectServiceUrl { get; }

        protected override bool CanSelectApiVersion => false;

        protected override string? ApiVersion => Client?.ApiVersion ?? base.ApiVersion;
        
        protected override IForecastService PrepareClient(CancellationToken cancellationToken) =>
            _serviceFactory((ServiceBaseUrl, ApiVersion), cancellationToken);
        
        protected override async Task<IReadOnlyCollection<Guid>> GetTodayForecasts(CancellationToken cancellationToken)
        {
            UserInterface.AppendInfoLine($"{nameof(Client.GetTodayForecasts)}({GetApiVersionInfo()}) ...");

            var (success, data) = await CallAsync(ct => Client.GetTodayForecasts(ct), cancellationToken);
            if (!success)
            {
                return Array.Empty<Guid>();
            }

            UserInterface.AppendInfoLine($"Found {data?.Count} forecasts");
            var ids = data?.Select(d => d.Id).ToArray();
            return ids ?? Array.Empty<Guid>();
        }

        protected override async Task<Guid?> FindForecast(Guid forecastId, CancellationToken cancellationToken)
        {
            UserInterface.AppendInfoLine($"{nameof(Client.FindForecast)}({forecastId}, {GetApiVersionInfo()}) ...");

            var (success, data) = await CallAsync(ct => Client.FindForecast(forecastId, ct), cancellationToken);
            return success ? data?.Id : null;
        }

        protected override async Task<Guid?> AddComment(Guid forecastId, string comment, CancellationToken cancellationToken)
        {
            UserInterface.AppendInfoLine($"{nameof(Client.AddComment)}({forecastId}, \"{comment}\", {GetApiVersionInfo()}) ...");

            var (success, data) = await CallAsync(ct => Client.AddComment(forecastId, new ForecastCommentDataDto(comment), ct), cancellationToken);
            return success ? data?.Id : null;
        }

        protected override async Task<Guid?> GetComment(Guid forecastId, Guid commentId, CancellationToken cancellationToken)
        {
            UserInterface.AppendInfoLine($"{nameof(Client.FindForecast)}({forecastId}, {commentId}, {GetApiVersionInfo()}) ...");

            var (success, data) = await CallAsync(ct => Client.GetComment(forecastId, commentId, ct), cancellationToken);
            return success ? data?.Id : null;
        }

        protected override async Task<IReadOnlyCollection<Guid>> GetForecastComments(Guid forecastId, CancellationToken cancellationToken)
        {
            UserInterface.AppendInfoLine($"{nameof(Client.GetForecastComments)}({forecastId}, {GetApiVersionInfo()}) ...");

            var (success, data) = await CallAsync(ct => Client.GetForecastComments(forecastId, ct), cancellationToken);
            if (!success)
            {
                return new Guid[0];
            }

            var list = data.Select(d => d.Id).ToArray();
            UserInterface.AppendInfoLine($"Found {list.Length} forecast comments");
            var ids = list.Select(id => id).ToArray();
            return ids;
        }

        protected override async Task<Guid?> UpdateComment(Guid forecastId, Guid commentId, string comment, CancellationToken cancellationToken)
        {
            UserInterface.AppendInfoLine($"{nameof(Client.UpdateComment)}({forecastId}, {commentId}, '{comment}', {GetApiVersionInfo()}) ...");

            var (success, data) = await CallAsync(ct => Client.UpdateComment(forecastId, commentId, new ForecastCommentDataDto(comment), ct), cancellationToken);
            return success ? data?.Id : null;
        }

        protected override async Task<IReadOnlyCollection<Guid>> GetLocationForecasts(string location, CancellationToken cancellationToken)
        {
            UserInterface.AppendInfoLine($"{nameof(Client.GetLocationForecasts)}('{location}', {GetApiVersionInfo()}) ...");

            var (success, data) = await CallAsync(ct => Client.GetLocationForecasts(location, ct), cancellationToken);
            if (!success)
            {
                return Array.Empty<Guid>();
            }

            UserInterface.AppendInfoLine($"Found {data?.Count} forecasts");
            var ids = data?.Select(d => d.Id).ToArray();
            return ids ?? Array.Empty<Guid>();
        }

        private async Task<(bool success, T data)> CallAsync<T>(Func<CancellationToken, Task<CallResult<T>>> serviceCall, CancellationToken cancellationToken)
        {
            var result = await serviceCall(cancellationToken);
            switch (result.ResultCode)
            {
                case ResultCode.Ok:
                    var data = result.Result;
                    await ShowSerializedData(data, cancellationToken);
                    return (true, data);

                default:
                    await UserInterface.ShowWarning($"Service response: {result.ResultCode}: {result.Description}", cancellationToken);
                    return (false, default)!;
            }
        }
    }
}
