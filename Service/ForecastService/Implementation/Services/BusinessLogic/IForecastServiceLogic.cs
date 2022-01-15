using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ForecastService.Services.BusinessLogic.Model;

namespace ForecastService.Services.BusinessLogic
{
    public interface IForecastServiceLogic
    {
        /// <summary>
        /// Returns today's forecasts for all cities
        /// </summary>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<IReadOnlyCollection<WeatherForecast>?> GetTodayForecasts(CancellationToken cancellationToken);

        /// <summary>
        /// Returns forecasts for defined location
        /// </summary>
        /// <param name="location">location name</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<IReadOnlyCollection<WeatherForecast>?> GetLocationForecasts(string location, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether specified forecats is available
        /// </summary>
        /// <param name="id">Forecast id</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<bool> IsForecastAvailable(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Returns specified forecast
        /// </summary>
        /// <param name="id">Forecast id</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<WeatherForecast?> FindForecast(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Returns comments for forecast
        /// </summary>
        /// <param name="forecastId">Forecast id</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<IReadOnlyCollection<ForecastComment>?> GetForecastComments(Guid forecastId, CancellationToken cancellationToken);

        /// <summary>
        /// Creates new comment
        /// </summary>
        /// <param name="forecastId">Forecast id</param>
        /// <param name="forecastCommentData">Comment request</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<ForecastComment?> AddComment(Guid forecastId, CommentData forecastCommentData, CancellationToken cancellationToken);

        /// <summary>
        /// Returns specified comment
        /// </summary>
        /// <param name="forecastId">Forecast id</param>
        /// <param name="commentId">Comment id</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<ForecastComment?> GetComment(Guid forecastId, Guid commentId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates comment
        /// </summary>
        /// <param name="forecastId">Forecast id</param>
        /// <param name="commentId">comment id</param>
        /// <param name="commentData">Comment</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<ForecastComment?> UpdateComment(Guid forecastId, Guid commentId, CommentData commentData, CancellationToken cancellationToken);
    }
}