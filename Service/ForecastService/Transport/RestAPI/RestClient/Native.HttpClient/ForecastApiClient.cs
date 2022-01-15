using System;
using ForecastService.Interface;
using JetBrains.Annotations;

namespace ForecastClient.Native.HttpClient
{
    [PublicAPI]
    public class ForecastApiClient : ForecastClientBase
    {
        public ForecastApiClient(Uri serviceUrl)
            : base(serviceUrl, ServiceInfo.ApiVersion)
        {
        }

        public ForecastApiClient(Uri serviceUrl, System.Net.Http.HttpClient httpClient)
            : base(httpClient, serviceUrl, ServiceInfo.ApiVersion)
        {
        }
    }
}
