using JetBrains.Annotations;

namespace ForecastService.Interface
{
    [PublicAPI]
    public static class ServiceInfo
    {
        public static string ApiVersion => "2.1";
    }
}
