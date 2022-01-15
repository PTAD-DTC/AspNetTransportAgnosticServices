using System.Collections.Generic;
using ForecastService.Core;
using ForecastService.DtoBlMapper.Extensions;
using ForecastService.Interface.Models.DTO;
using ForecastService.Services.BusinessLogic.Model;

namespace ForecastService.DtoBlMapper
{
    public sealed class DtoBlMapper : IDtoBlMapper
    {
        public WeatherForecastDto? MapToDto(WeatherForecast? forecast)
        {
            return forecast.MapToDto_Safe();
        }

        public ForecastCommentDto? MapToDto(ForecastComment? comment)
        {
            return comment.MapToDto_Safe();
        }

        public CommentData? MapToBl(ForecastCommentDataDto? commentData)
        {
            return commentData.MapToBl_Safe();
        }

        public IEnumerable<WeatherForecastDto> MapToDto(IEnumerable<WeatherForecast> data)
        {
            return data.MapToDto();
        }

        public IEnumerable<ForecastCommentDto> MapToDto(IEnumerable<ForecastComment> data)
        {
            return data.MapToDto();
        }
    }
}
