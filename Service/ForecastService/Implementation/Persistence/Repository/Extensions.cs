using System;
using ForecastService.Persistence.Repository.Model;
using ForecastService.Services.BusinessLogic.Model;

namespace ForecastService.Persistence.Repository
{
    internal static class Extensions
    {
        public static ForecastProbability MapToBl(this ProbabilityData probability)
        {
            return probability switch
            {
                ProbabilityData.Guaranteed => ForecastProbability.Guaranteed,
                ProbabilityData.High => ForecastProbability.High,
                ProbabilityData.Low => ForecastProbability.Low,
                ProbabilityData.Guessing => ForecastProbability.Guessing,
                _ => throw new ArgumentOutOfRangeException(nameof(probability), probability, null)
            };
        }

        public static WeatherForecast MapToBl(this ForecastData forecastData)
        {
            return new WeatherForecast(
                forecastData.Id,
                forecastData.Date,
                forecastData.Probability.MapToBl()
,
                forecastData.Location,
                forecastData.TemperatureCelsius,
                forecastData.Summary,
                forecastData.Description);
        }

        public static WeatherForecast? MapToBlSafe(this ForecastData? forecastData)
        {
            return forecastData is null ? null : forecastData.MapToBl();
        }
    }
}
