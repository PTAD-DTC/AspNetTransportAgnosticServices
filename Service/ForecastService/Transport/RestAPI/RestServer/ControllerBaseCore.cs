namespace ForecastService.RestServer
{
    /// <summary>
    /// Base class for Forecast service controllers 
    /// </summary>
    public abstract class ControllerBaseCore<TService> : Service.RestServer.Base.ControllerBaseCore<TService>
    where TService : class
    {
        /// <summary>
        /// ctor
        /// </summary>
        protected ControllerBaseCore(TService service, string? apiVersion = null)
            : base(service, apiVersion)
        {
        }
    }
}
