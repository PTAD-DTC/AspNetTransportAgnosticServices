using Client.Base;
using ForecastService.Interface;
using ForecastService.Interface.Models.DTO;
using JetBrains.Annotations;
using RestSharp;
using Service.Interface.Base;
using ServiceBaseClient.RestClient.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ForecastClient.Native.RestClient
{
    [PublicAPI]
    public abstract class ForecastClientBase : ServiceRestSharpClientBase, IForecastService
    {
        private const string UserAgentPrefix = "forecastsvc-nativerestsharpclient";
        private const string ClientVersion = "2.1";

        private static readonly string ServicePathBase = ForecastService.RestDefinitions.Definitions.ServicePathBase.ToUriComponent();
        private static readonly string RouteForecast = $"{ServicePathBase}/Forecast";
        private static readonly string RouteForecastToday = $"{RouteForecast}/today";
        private static readonly string RouteForecastLocation = $"{RouteForecast}/location";

        private static readonly string RouteComment = $"{ServicePathBase}/Comment";

        /// <summary>
        /// Constructor
        /// </summary>
        protected ForecastClientBase(IRestClient restClient, string apiVersion, ICommunicationDebugListener? communicationListener = null)
            : base(restClient, apiVersion, communicationListener)
        {
            UserAgentName = GetUserAgentName(ClientVersion);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected ForecastClientBase(Uri serviceBaseUrl, string apiVersion, ICommunicationDebugListener? communicationListener = null)
            : base(serviceBaseUrl, apiVersion, communicationListener)
        {
            UserAgentName = GetUserAgentName(ClientVersion);
        }

        protected override string ResponseApiVersion => ServiceInfo.ApiVersion;

        /// <inheritdoc />
        Task<CallResult<IReadOnlyCollection<WeatherForecastDto>?>> IForecastService.GetTodayForecasts(CancellationToken cancellationToken)
        {
            return CallGetServiceSafe<IReadOnlyCollection<WeatherForecastDto>?>(
                RouteForecastToday,
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<CallResult<IReadOnlyCollection<WeatherForecastDto>?>> GetTodayForecasts(CancellationToken cancellationToken)
        {
            return CallGetServiceSafe<IReadOnlyCollection<WeatherForecastDto>?>(
                RouteForecastToday,
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<CallResult<IReadOnlyCollection<WeatherForecastDto>?>> GetLocationForecasts(string location, CancellationToken cancellationToken)
        {
            return CallGetServiceSafe<IReadOnlyCollection<WeatherForecastDto>?>(
                $"{RouteForecastLocation}/{location}",
                cancellationToken);
        }

        /// <inheritdoc />
        Task<CallResult<WeatherForecastDto?>> IForecastService.FindForecast(Guid id, CancellationToken cancellationToken)
        {
            return CallGetServiceSafe<WeatherForecastDto?>(
                $"{RouteForecast}/{id}",
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<CallResult<WeatherForecastDto?>> FindForecast(Guid id, CancellationToken cancellationToken)
        {
            return CallGetServiceSafe<WeatherForecastDto?>(
                $"{RouteForecast}/{id}",
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<CallResult<IReadOnlyCollection<ForecastCommentDto>?>> GetForecastComments(Guid forecastId, CancellationToken cancellationToken)
        {
            return CallGetServiceSafe<IReadOnlyCollection<ForecastCommentDto>?>(
                $"{RouteComment}/{forecastId}",
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<CallResult<ForecastCommentDto?>> AddComment(Guid forecastId, ForecastCommentDataDto commentData, CancellationToken cancellationToken)
        {
            return CallPostServiceSafe<ForecastCommentDto?, ForecastCommentDataDto>(
                $"{RouteComment}/{forecastId}",
                commentData,
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<CallResult<ForecastCommentDto?>> GetComment(Guid forecastId, Guid commentId, CancellationToken cancellationToken)
        {
            return CallGetServiceSafe<ForecastCommentDto?>(
                $"{RouteComment}/{forecastId}/{commentId}",
                cancellationToken);
        }

        public Task<CallResult<ForecastCommentDto?>> UpdateComment(Guid forecastId, Guid commentId, ForecastCommentDataDto commentData, CancellationToken cancellationToken)
        {
            return CallPutServiceSafe<ForecastCommentDto?, ForecastCommentDataDto>(
                $"{RouteComment}/{forecastId}/{commentId}",
                commentData,
                cancellationToken);
        }

        protected override string UserAgentName { get; }

        private static string GetUserAgentName(string? clientVersion)
        {
            var clientVersionInfo = string.IsNullOrWhiteSpace(clientVersion) ? null : $"/{clientVersion}";
            return $"{UserAgentPrefix}{clientVersionInfo}";
        }
    }
}
