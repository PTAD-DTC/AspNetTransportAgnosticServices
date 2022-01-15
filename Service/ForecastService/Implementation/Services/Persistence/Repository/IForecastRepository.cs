using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ForecastService.Services.BusinessLogic.Model;

namespace ForecastService.Services.Persistence.Repository
{
    public interface IForecastRepository
    {
        Task<IReadOnlyCollection<WeatherForecast>> GetTodayForecasts(CancellationToken cancellationToken);

        Task<IReadOnlyCollection<WeatherForecast>> GetLocationForecasts(string location, CancellationToken cancellationToken);

        Task<bool> ContainsForecast(Guid id, CancellationToken cancellationToken);

        Task<WeatherForecast?> FindForecast(Guid id, CancellationToken cancellationToken);
    }
}
