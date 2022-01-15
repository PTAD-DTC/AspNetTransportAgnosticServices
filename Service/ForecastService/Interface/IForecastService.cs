using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ForecastService.Interface.Models.DTO;
using JetBrains.Annotations;
using Service.Interface.Base;

namespace ForecastService.Interface
{
    [PublicAPI]
    public interface IForecastService : IServiceBase
    {
        /// <summary>
        /// Returns today's forecasts for all cities
        /// </summary>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<CallResult<IReadOnlyCollection<WeatherForecastDto>?>> GetTodayForecasts(CancellationToken cancellationToken);

        /// <summary>
        /// Returns specified forecast
        /// </summary>
        /// <param name="id">Forecast id</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<CallResult<WeatherForecastDto?>> FindForecast(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Returns forecasts for defined location
        /// </summary>
        /// <param name="location">location name</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<CallResult<IReadOnlyCollection<WeatherForecastDto>?>> GetLocationForecasts(string location, CancellationToken cancellationToken);

        /// <summary>
        /// Returns comments for forecast
        /// </summary>
        /// <param name="forecastId">Forecast id</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<CallResult<IReadOnlyCollection<ForecastCommentDto>?>> GetForecastComments(Guid forecastId, CancellationToken cancellationToken);

        /// <summary>
        /// Creates new comment
        /// </summary>
        /// <param name="forecastId">Forecast id</param>
        /// <param name="commentData">Comment</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<CallResult<ForecastCommentDto?>> AddComment(Guid forecastId, ForecastCommentDataDto commentData, CancellationToken cancellationToken);

        /// <summary>
        /// Returns specified comment
        /// </summary>
        /// <param name="forecastId">Forecast id</param>
        /// <param name="commentId">Comment id</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<CallResult<ForecastCommentDto?>> GetComment(Guid forecastId, Guid commentId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates comment
        /// </summary>
        /// <param name="forecastId">Forecast id</param>
        /// <param name="commentId">comment id</param>
        /// <param name="commentData">Comment</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<CallResult<ForecastCommentDto?>> UpdateComment(Guid forecastId, Guid commentId, ForecastCommentDataDto commentData, CancellationToken cancellationToken);
    }
}