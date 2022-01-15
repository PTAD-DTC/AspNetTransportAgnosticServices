using System;

namespace ForecastService.Services.BusinessLogic.Model
{
    public sealed class WeatherForecast
    {
        public WeatherForecast(Guid id, DateTime date, ForecastProbability probability, string location, int temperatureC, string summary, string description)
        {
            Id = id;
            Location = location;
            Date = date;
            TemperatureC = temperatureC;
            Summary = summary;
            Description = description;
            Probability = probability;
        }

        public Guid Id { get; }

        public DateTime Date { get; }

        public ForecastProbability Probability { get; }

        public string Location { get; }

        public int TemperatureC { get; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; }

        public string Description { get; }
    }
}
