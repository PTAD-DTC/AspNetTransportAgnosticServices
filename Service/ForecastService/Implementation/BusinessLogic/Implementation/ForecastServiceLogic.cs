using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ForecastService.Services.BusinessLogic;
using ForecastService.Services.BusinessLogic.Model;
using ForecastService.Services.CommentService;
using ForecastService.Services.Persistence;

namespace ForecastService.BusinessLogic.Implementation
{
    public class ForecastServiceLogic : IForecastServiceLogic
    {
        private readonly IForecastDataStorage _dataStorage;
        private readonly IForecastCommentService _commentService;

        public ForecastServiceLogic(IForecastDataStorage dataStorage, IForecastCommentService commentService)
        {
            _dataStorage = dataStorage ?? throw new ArgumentNullException(nameof(dataStorage));
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<WeatherForecast>?> GetTodayForecasts(CancellationToken cancellationToken)
        {
            var data = await _dataStorage.ForecastRepository.GetTodayForecasts(cancellationToken);
            return data;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<WeatherForecast>?> GetLocationForecasts(string location, CancellationToken cancellationToken)
        {
            var data = await _dataStorage.ForecastRepository.GetLocationForecasts(location, cancellationToken);
            return data;
        }

        /// <inheritdoc />
        public async Task<bool> IsForecastAvailable(Guid id, CancellationToken cancellationToken)
        {
            var contains = await _dataStorage.ForecastRepository.ContainsForecast(id, cancellationToken);
            return contains;
        }

        /// <inheritdoc />
        public async Task<WeatherForecast?> FindForecast(Guid id, CancellationToken cancellationToken)
        {
            var forecast = await _dataStorage.ForecastRepository.FindForecast(id, cancellationToken);
            return forecast;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<ForecastComment>?> GetForecastComments(Guid forecastId, CancellationToken cancellationToken)
        {
            var containsForecast = await _dataStorage.ForecastRepository.ContainsForecast(forecastId, cancellationToken);
            if (!containsForecast)
            {
                return null;
            }

            var comments = await _commentService.GetComments(forecastId, cancellationToken);
            return comments;
        }

        /// <inheritdoc />
        public async Task<ForecastComment?> AddComment(Guid forecastId, CommentData forecastCommentData, CancellationToken cancellationToken)
        {
            if (forecastCommentData is null)
            {
                throw new ArgumentNullException(nameof(forecastCommentData));
            }

            var contains = await _dataStorage.ForecastRepository.ContainsForecast(forecastId, cancellationToken);
            if (!contains)
            {
                return null;
            }

            var comment = await _commentService.AddComment(forecastId, forecastCommentData, cancellationToken);
            return comment;
        }

        /// <inheritdoc />
        public async Task<ForecastComment?> GetComment(Guid forecastId, Guid id, CancellationToken cancellationToken)
        {
            var contains = await _dataStorage.ForecastRepository.ContainsForecast(forecastId, cancellationToken);
            if (!contains)
            {
                return null;
            }

            var comment = await _commentService.GetComment(id, cancellationToken);
            return comment;
        }

        /// <inheritdoc />
        public async Task<ForecastComment?> UpdateComment(Guid forecastId, Guid id, CommentData commentData, CancellationToken cancellationToken)
        {
            if (commentData is null)
            {
                throw new ArgumentNullException(nameof(commentData));
            }

            var contains = await _dataStorage.ForecastRepository.ContainsForecast(forecastId, cancellationToken);
            if (!contains)
            {
                return null;
            }

            var comment = await _commentService.UpdateComment(id, commentData, cancellationToken);
            return comment;
        }
    }
}
