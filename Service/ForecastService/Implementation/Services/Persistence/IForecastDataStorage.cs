using ForecastService.Services.Persistence.Repository;

namespace ForecastService.Services.Persistence
{
    public interface IForecastDataStorage
    {
        IForecastRepository ForecastRepository { get; }
    }
}
