using System;
using ForecastService.Interface;
using JetBrains.Annotations;
using RestSharp;

namespace ForecastClient.Native.RestClient
{
    [PublicAPI]
    public sealed class ForecastApiClient : ForecastClientBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ForecastApiClient(IRestClient restClient)
            : base(restClient, ServiceInfo.ApiVersion)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ForecastApiClient(Uri serviceBaseUrl)
            : base(serviceBaseUrl, ServiceInfo.ApiVersion)
        {
        }
    }
}
