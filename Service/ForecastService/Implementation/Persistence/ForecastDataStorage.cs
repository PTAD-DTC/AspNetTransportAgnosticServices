using System;
using ForecastService.Services.Persistence;
using ForecastService.Services.Persistence.Repository;

namespace ForecastService.Persistence
{
    public sealed class ForecastDataStorage : IForecastDataStorage
    {
        public ForecastDataStorage(IForecastRepository forecastRepository)
        {
            ForecastRepository = forecastRepository ?? throw new ArgumentNullException(nameof(forecastRepository));
        }

        public IForecastRepository ForecastRepository { get; }
    }
}
