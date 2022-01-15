using System.Collections.Generic;
using ForecastService.Interface.Models.DTO;
using ForecastService.Services.BusinessLogic.Model;
using Helpers;

namespace ForecastService.DtoBlMapper.Extensions
{
    internal static class DtoMapper
    {
        private static ForecastProbabilityDto MapToDto(this ForecastProbability probability)
        {
            return probability switch
            {
                ForecastProbability.Guaranteed => ForecastProbabilityDto.Guaranteed,
                ForecastProbability.High => ForecastProbabilityDto.High,
                ForecastProbability.Low => ForecastProbabilityDto.Low,
                ForecastProbability.Guessing => ForecastProbabilityDto.Low,
                _ => ForecastProbabilityDto.NotDefined,
            };
        }

        public static WeatherForecastDto MapTo(this WeatherForecast forecast)
        {
            return new WeatherForecastDto(
                forecast.Id,
                forecast.Date,
                forecast.Location,
                forecast.TemperatureC,
                forecast.TemperatureF,
                forecast.Summary,
                forecast.Description,
                forecast.Probability.MapToDto());
        }

        public static WeatherForecastDto MapToDto(this WeatherForecast forecast)
        {
            return new WeatherForecastDto(
                forecast.Id,
                forecast.Date,
                forecast.Location,
                forecast.TemperatureC,
                forecast.TemperatureF,
                forecast.Summary,
                forecast.Description,
                forecast.Probability.MapToDto());
        }

        public static WeatherForecastDto? MapToDto_Safe(this WeatherForecast? forecast)
        {
            return forecast?.MapToDto();
        }

        public static ForecastCommentDto MapToDto(this ForecastComment comment)
        {
            return new ForecastCommentDto(
                comment.Id,
                comment.ForecastId,
                comment.Comment.MapToDto());
        }

        public static ForecastCommentDto? MapToDto_Safe(this ForecastComment? comment)
        {
            return comment?.MapToDto();
        }

        private static ForecastCommentDataDto MapToDto(this CommentData comment)
        {
            return new ForecastCommentDataDto(comment.Comment);
        }

        public static IEnumerable<WeatherForecastDto> MapToDto(this IEnumerable<WeatherForecast> data)
        {
            return data.MapTo(MapToDto)!;
        }

        public static IEnumerable<ForecastCommentDto> MapToDto(this IEnumerable<ForecastComment> data)
        {
            return data.MapTo(MapToDto);
        }
    }
}
