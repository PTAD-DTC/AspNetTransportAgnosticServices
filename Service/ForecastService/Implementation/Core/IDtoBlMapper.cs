using System.Collections.Generic;
using ForecastService.Interface.Models.DTO;
using ForecastService.Services.BusinessLogic.Model;

namespace ForecastService.Core
{
    public interface IDtoBlMapper
    {
        WeatherForecastDto? MapToDto(WeatherForecast? forecast);

        ForecastCommentDto? MapToDto(ForecastComment? comment);

        CommentData? MapToBl(ForecastCommentDataDto? commentData);

        IEnumerable<WeatherForecastDto> MapToDto(IEnumerable<WeatherForecast> data);

        IEnumerable<ForecastCommentDto> MapToDto(IEnumerable<ForecastComment> data);
    }
}