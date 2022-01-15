using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForecastService.RestDefinitions
{
    public static class Definitions
    {
        public static readonly PathString ServicePathBase = "/api/forecastservice";
        public const string ServiceName = "Forecast Service";
#if DEBUG
        public static readonly ApiVersion? DefaultApiVersion = new ApiVersion(2, 1);
#else
        public static readonly ApiVersion? DefaultApiVersion = null;
#endif
    }
}
