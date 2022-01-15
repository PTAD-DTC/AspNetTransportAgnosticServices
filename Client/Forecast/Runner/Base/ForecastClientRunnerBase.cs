using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Runner.Base;

namespace ForecastClient.Runner.Base
{
    public abstract class ForecastClientRunnerBase<TServiceClient> : ClientRunnerBase<TServiceClient>
    {
        protected ForecastClientRunnerBase(IClientRunnerUserInterface userInterface) : base(userInterface)
        {
        }

        protected Guid? SelectedForecast { get; set; }

        protected Guid? SelectedComment { get; set; }

        protected bool CanCallGetTodayForecasts => Client is { };
        protected bool CanCallFindForecast => Client is { } && SelectedForecast.HasValue;
        protected bool CanCallGetForecastComments => Client is { } && SelectedForecast.HasValue;
        protected bool CanCallAddComment => Client is { } && SelectedForecast.HasValue;
        protected bool CanCallGetComment => Client is { } && SelectedForecast.HasValue && SelectedComment.HasValue;
        protected bool CanCallUpdateComment => Client is { } && SelectedForecast.HasValue && SelectedComment.HasValue;
        protected bool CanCallGetLocationForecasts => Client is { };

        protected override void ClearData()
        {
            SelectedComment = null;
        }

        protected override IEnumerable<(ConsoleKey? key, string? name, Func<CancellationToken, Task<bool>> operation)> GetCurrentOperationChoices()
        {
            if (CanCallGetTodayForecasts)
            {
                yield return (ConsoleKey.A, nameof(GetTodayForecasts), DoGetTodayForecasts);
            }

            if (CanCallFindForecast)
            {
                yield return (ConsoleKey.B, $"{nameof(FindForecast)} {SelectedForecast}", DoFindForecast);
            }

            yield return (null, null, ct => Task.FromResult(false));

            if (CanCallGetForecastComments)
                yield return (ConsoleKey.C, $"{nameof(GetForecastComments)} {SelectedForecast}", DoGetForecastComments);
            if (CanCallGetComment)
                yield return (ConsoleKey.D, $"{nameof(GetComment)} {SelectedComment}", DoGetComment);
            if (CanCallAddComment)
                yield return (ConsoleKey.E, $"{nameof(AddComment)} {SelectedComment}", DoAddComment);
            if (CanCallUpdateComment)
                yield return (ConsoleKey.F, $"{nameof(UpdateComment)} {SelectedComment}", DoUpdateComment);

            yield return (null, null, ct => Task.FromResult(false));

            if (CanCallGetLocationForecasts)
                yield return (ConsoleKey.G, $"{nameof(GetLocationForecasts)}", DoGetLocationForecasts);
        }

        protected abstract Task<IReadOnlyCollection<Guid>> GetTodayForecasts(CancellationToken cancellationToken);

        protected abstract Task<IReadOnlyCollection<Guid>> GetForecastComments(Guid forecastId,
            CancellationToken cancellationToken);

        protected abstract Task<Guid?> FindForecast(Guid forecastId, CancellationToken cancellationToken);

        protected abstract Task<Guid?> AddComment(Guid forecastId, string comment, CancellationToken cancellationToken);

        protected abstract Task<Guid?> GetComment(Guid forecastId, Guid commentId, CancellationToken cancellationToken);

        protected abstract Task<Guid?> UpdateComment(Guid forecastId, Guid commentId, string comment,
            CancellationToken cancellationToken);

        protected abstract Task<IReadOnlyCollection<Guid>> GetLocationForecasts(string location, CancellationToken cancellationToken);

        private async Task<bool> DoGetTodayForecasts(CancellationToken cancellationToken)
        {
            await DoOperation(nameof(GetTodayForecasts),
                async ct =>
                {
                    UserInterface.AppendInfoLine(
                        $"Executing {nameof(DoGetTodayForecasts)} with params ({GetApiVersionInfo()})");

                    var forecasts = await GetTodayForecasts(ct);
                    var ids = forecasts?.Where(id => id != Guid.Empty).Select(id => id).ToArray();
                    SelectedForecast = ids?.Length > 0 ? ids[0] : (Guid?)null;
                    UserInterface.AppendInfoLine($"Found {ids?.Length ?? 0} valid forecast ids");
                },
                cancellationToken);

            return true;
        }

        private async Task<bool> DoFindForecast(CancellationToken cancellationToken)
        {
            if (!SelectedForecast.HasValue)
            {
                return true;
            }

            await DoOperation(nameof(FindForecast),
                async ct =>
                {
                    UserInterface.AppendInfoLine(
                        $"Executing {nameof(FindForecast)}({GetApiVersionInfo()}, {SelectedForecast})");

                    var id = await FindForecast(SelectedForecast.Value, ct);
                    UserInterface.AppendInfoLine((id.HasValue && id.Value != Guid.Empty)
                        ? $"Found forecast {id.Value}"
                        : "Forecast not found");
                },
                cancellationToken);

            return true;
        }

        private async Task<bool> DoGetForecastComments(CancellationToken cancellationToken)
        {
            if (!SelectedForecast.HasValue)
            {
                return true;
            }

            await DoOperation(nameof(GetForecastComments),
                async ct =>
                {
                    UserInterface.AppendInfoLine(
                        $"Executing {nameof(GetForecastComments)}({GetApiVersionInfo()}, {SelectedForecast})");

                    var comments = await GetForecastComments(SelectedForecast.Value, ct);
                    var ids = comments?.Where(id => id != Guid.Empty).Select(id => id).ToArray();
                    SelectedComment = ids?.Length > 0 ? ids[0] : (Guid?)null;
                    UserInterface.AppendInfoLine($"Found {ids?.Length ?? 0} valid comment ids");
                },
                cancellationToken);

            return true;
        }

        private async Task<bool> DoAddComment(CancellationToken cancellationToken)
        {
            if (!SelectedForecast.HasValue)
            {
                return true;
            }

            await DoOperation(nameof(AddComment),
                async ct =>
                {
                    var comment = await UserInterface.GetUserInfo("Enter comment: ", ct) ?? string.Empty;
                    UserInterface.AppendInfoLine(
                        $"Executing {nameof(AddComment)}({GetApiVersionInfo()}, {SelectedForecast}, \"{comment}\")");
                    var id = await AddComment(SelectedForecast.Value, comment, ct);
                    UserInterface.AppendInfoLine((id.HasValue && id.Value != Guid.Empty)
                        ? $"Comment {id.Value} added"
                        : "Comment not added");
                    SelectedComment = id ?? SelectedComment;
                },
                cancellationToken);

            return true;
        }

        private async Task<bool> DoGetComment(CancellationToken cancellationToken)
        {
            if (!SelectedForecast.HasValue || !SelectedComment.HasValue)
            {
                return true;
            }

            await DoOperation(nameof(GetComment),
                async ct =>
                {
                    UserInterface.AppendInfoLine(
                        $"Executing {nameof(GetComment)}({SelectedForecast}, {SelectedComment}, {GetApiVersionInfo()})");

                    var id = await GetComment(SelectedForecast.Value, SelectedComment.Value, ct);
                    UserInterface.AppendInfoLine((id.HasValue && id.Value != Guid.Empty)
                        ? $"Found comment {id.Value}"
                        : "Comment not found");
                },
                cancellationToken);

            return true;
        }

        private async Task<bool> DoUpdateComment(CancellationToken cancellationToken)
        {
            if (!SelectedForecast.HasValue || !SelectedComment.HasValue)
            {
                return true;
            }

            await DoOperation(nameof(UpdateComment),
                async ct =>
                {
                    var comment = await UserInterface.GetUserInfo("Enter comment: ", ct) ?? string.Empty;
                    UserInterface.AppendInfoLine(
                        $"Executing {nameof(UpdateComment)}({SelectedForecast}, {SelectedComment}, \"{comment}\", {GetApiVersionInfo()})");
                    var id = await UpdateComment(SelectedForecast.Value, SelectedComment.Value, comment,
                        cancellationToken);
                    UserInterface.AppendInfoLine((id.HasValue && id.Value != Guid.Empty)
                        ? $"Comment {id.Value} updated"
                        : "Comment not updated");
                },
                cancellationToken);

            return true;
        }

        private async Task<bool> DoGetLocationForecasts(CancellationToken cancellationToken)
        {
            await DoOperation(nameof(GetLocationForecasts),
                async ct =>
                {
                    var location = await UserInterface.GetUserInfo("Enter location: ", ct) ?? string.Empty;
                    UserInterface.AppendInfoLine($"Executing {nameof(GetLocationForecasts)} with params ('{location}', {GetApiVersionInfo()})");
                    var forecasts = await GetLocationForecasts(location, ct);
                    var ids = forecasts?.Where(id => id != Guid.Empty).Select(id => id).ToArray();
                    SelectedForecast = ids?.Length > 0 ? ids[0] : (Guid?)null;
                    UserInterface.AppendInfoLine($"Found {ids?.Length ?? 0} valid forecast ids");
                },
                cancellationToken);

            return true;
        }
    }
}
