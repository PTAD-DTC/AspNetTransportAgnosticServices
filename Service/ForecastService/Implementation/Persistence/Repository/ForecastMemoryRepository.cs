using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ForecastService.Persistence.Repository.Model;
using ForecastService.Services.BusinessLogic.Model;
using ForecastService.Services.Persistence.Repository;

namespace ForecastService.Persistence.Repository
{
    public sealed class ForecastMemoryRepository : IForecastRepository
    {
        private static readonly ConcurrentDictionary<Guid, ForecastData> Storage =
            new(GetSampleForecasts().Select(s => KeyValuePair.Create(s.Id, s)));

        private static IEnumerable<ForecastData> GetSampleForecasts()
        {
            var rng = new Random();
            var sampleSummaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            ForecastData CreateData(int index, string location) =>
                new(
                    Guid.NewGuid(),
                    DateTime.UtcNow.Date.AddDays(index),
                    GetProbability(index),
                    location,
                    rng.Next(-20, 55),
                    sampleSummaries[rng.Next(sampleSummaries.Length)],
                    "Waiting for editor");

            return Enumerable.Range(0, 3).SelectMany(index => new[]
            {
                CreateData(index, "England"),
                CreateData(index, "Portugal"),
                CreateData(index, "Spain"),
                CreateData(index, "France"),
                CreateData(index, "Belgium"),
                CreateData(index, "Netherlands"),
                CreateData(index, "Switzerland"),
                CreateData(index, "Austria"),
                CreateData(index, "Poland"),
                CreateData(index, "Ukraine"),
                CreateData(index, "Belarus"),
                CreateData(index, "Russia"),
                CreateData(index, "China"),
                CreateData(index, "Japan"),
                CreateData(index, "India"),
                CreateData(index, "Canada"),
                CreateData(index, "Us"),
            });

            static ProbabilityData GetProbability(int index)
            {
                return index switch
                {
                    <= 1 => ProbabilityData.Guaranteed,
                    2 => ProbabilityData.High,
                    3 => ProbabilityData.Low,
                    _ => ProbabilityData.Guessing
                };
            }
        }

        public Task<IReadOnlyCollection<WeatherForecast>> GetTodayForecasts(CancellationToken cancellationToken)
        {
            var raw = Storage.ToArray();

            var today = DateTime.UtcNow.Date;
            var data = raw
                .Where(f => f.Value.Date.Date == today)
                .OrderBy(f => f.Value.Date)
                .Select(f => f.Value.MapToBl())
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<WeatherForecast>>(data);
        }

        public Task<IReadOnlyCollection<WeatherForecast>> GetLocationForecasts(string location, CancellationToken cancellationToken)
        {
            var raw = Storage.Where(f => string.Equals(f.Value.Location, location, StringComparison.OrdinalIgnoreCase)).ToArray();

            var data = raw
                .Where(f => string.Equals(location, f.Value.Location, StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f.Value.Date)
                .Select(f => f.Value.MapToBl())
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<WeatherForecast>>(data);
        }

        public Task<bool> ContainsForecast(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(Storage.ContainsKey(id));

        public Task<WeatherForecast?> FindForecast(Guid id, CancellationToken cancellationToken)
        {
            var success = Storage.TryGetValue(id, out var forecast);

            return Task.FromResult(success ? forecast.MapToBlSafe() : null);
        }
    }
}
