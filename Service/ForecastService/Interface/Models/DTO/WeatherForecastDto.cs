using System;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Service.Interface.Base;

namespace ForecastService.Interface.Models.DTO
{
    /// <summary>
    /// Weather forecast
    /// </summary>
    [PublicAPI]
    // ReSharper disable once InconsistentNaming
    public sealed class WeatherForecastDto : ModelBase
    {
        private WeatherForecastDto()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public WeatherForecastDto(Guid id, DateTime date, string location, int temperatureC, int? temperatureF, string summary, string description, ForecastProbabilityDto probability)
        {
            Id = id;
            Date = date;
            TemperatureC = temperatureC;
            TemperatureF = temperatureF;
            Summary = summary;
            Description = description;
            Probability = probability;
            Location = location;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Temperature (Celsius)
        /// </summary>
        [JsonPropertyName("celsius")]
        public int TemperatureC { get; set; }

        /// <summary>
        /// Temperature (Fahrenheit)
        /// </summary>
        [JsonPropertyName("fahrenheit")]
        public int? TemperatureF { get; set; }

        /// <summary>
        /// Short summary
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [JsonPropertyName("info")]
        public string? Description { get; set; }

        /// <summary>
        /// Probability
        /// </summary>
        [JsonPropertyName("chance")]
        public ForecastProbabilityDto Probability { get; set; }
    }
}
