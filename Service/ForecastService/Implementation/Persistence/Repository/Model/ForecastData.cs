using System;

namespace ForecastService.Persistence.Repository.Model
{
    internal sealed class ForecastData
    {
        public ForecastData(Guid id, DateTime date, ProbabilityData probability, string location, int temperatureCelsius, string summary, string description)
        {
            Id = id;
            Date = date.Date;
            Probability = probability;
            Location = location;
            TemperatureCelsius = temperatureCelsius;
            Summary = summary;
            Description = description;
        }

        public Guid Id { get; }

        public DateTime Date { get; }

        public ProbabilityData Probability { get; }

        public string Location { get; }

        public int TemperatureCelsius { get; }

        public string Summary { get; }

        public string Description { get; }
    }
}
