using ForecastService.Interface;

namespace ForecastService.RestServer.V2_1.Controllers
{
    /// <summary>
    /// Base class for WeatherService controllers 
    /// </summary>
    public abstract class ControllerBaseForecastService : ControllerBaseCore<IForecastService>
    {
        /// <summary>
        /// ctor
        /// </summary>
        protected ControllerBaseForecastService(IForecastService forecastService) : base(forecastService, ServiceInfo.ApiVersion)
        {
        }
    }
}
