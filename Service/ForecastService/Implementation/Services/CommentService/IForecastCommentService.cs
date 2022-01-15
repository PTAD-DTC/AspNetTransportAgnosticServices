using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ForecastService.Services.BusinessLogic.Model;

namespace ForecastService.Services.CommentService
{
    public interface IForecastCommentService
    {
        /// <summary>
        /// Returns comments for forecast
        /// </summary>
        /// <param name="forecastId">Forecast id</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<IReadOnlyCollection<ForecastComment>?> GetComments(Guid forecastId, CancellationToken cancellationToken);

        /// <summary>
        /// Returns specified comment
        /// </summary>
        /// <param name="id">Comment id</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<ForecastComment?> GetComment(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Creates new comment
        /// </summary>
        /// <param name="forecastId">Forecast id</param>
        /// <param name="commentData">Comment request</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<ForecastComment?> AddComment(Guid forecastId, CommentData commentData, CancellationToken cancellationToken);

        /// <summary>
        /// Updates comment
        /// </summary>
        /// <param name="id">comment id</param>
        /// <param name="commentData">Comment</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<ForecastComment?> UpdateComment(Guid id, CommentData commentData, CancellationToken cancellationToken);
    }
}
