using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ForecastService.Interface;
using ForecastService.Interface.Models.DTO;
using ForecastService.Services.BusinessLogic;
using ForecastService.Services.BusinessLogic.Model;
using Service.Interface.Base;

namespace ForecastService.Core
{
    public sealed class ForecastServiceCore : ServiceCoreBase, IForecastService
    {
        public ForecastServiceCore(IForecastServiceLogic forecastServiceLogic, IDtoBlMapper dtoBlMapper) 
            : base(ServiceInfo.ApiVersion)
        {
            ForecastServiceLogic = forecastServiceLogic ?? throw new ArgumentNullException(nameof(forecastServiceLogic));
            DtoBlMapper = dtoBlMapper ?? throw new ArgumentNullException(nameof(dtoBlMapper));
        }

        private IForecastServiceLogic ForecastServiceLogic { get; }
        private IDtoBlMapper DtoBlMapper { get; }

        /// <inheritdoc cref="IForecastService" />
        public async Task<CallResult<IReadOnlyCollection<WeatherForecastDto>?>> GetTodayForecasts(CancellationToken cancellationToken)
        {
            var forecasts = await ForecastServiceLogic.GetTodayForecasts(cancellationToken);
            return CallResult(forecasts);

            //Transport benchmark
            var forecastsGenCount = 5000;
            var generated = Enumerable.Repeat(
                new WeatherForecast(
                    Guid.Empty,
                    DateTime.UtcNow.Date,
                    ForecastProbability.Low,
                    "Here",
                    10,
                    "summary",
                    "description"),
                forecastsGenCount).ToArray();
            await Task.CompletedTask;
            return CallResult(generated);
        }

        /// <inheritdoc cref="IForecastService" />
        public async Task<CallResult<WeatherForecastDto?>> FindForecast(Guid id, CancellationToken cancellationToken)
        {
            var forecast = await ForecastServiceLogic.FindForecast(id, cancellationToken);

            return CallResult(forecast);
        }

        /// <inheritdoc cref="IForecastService" />
        public async Task<CallResult<IReadOnlyCollection<ForecastCommentDto>?>> GetForecastComments(Guid forecastId, CancellationToken cancellationToken)
        {
            var comments = await ForecastServiceLogic.GetForecastComments(forecastId, cancellationToken);

            return CallResult(comments);
        }

        /// <inheritdoc cref="IForecastService" />
        public async Task<CallResult<ForecastCommentDto?>> AddComment(Guid forecastId, ForecastCommentDataDto commentData, CancellationToken cancellationToken)
        {
            if (commentData is null)
            {
                return BadRequest<ForecastCommentDto?>($"{nameof(commentData)} is null");
            }

            var blCommentData = DtoBlMapper.MapToBl(commentData);
            if (blCommentData is null)
            {
                return BadRequest<ForecastCommentDto?>($"{nameof(commentData)} is invalid");
            }

            var comment = await ForecastServiceLogic.AddComment(forecastId, blCommentData, cancellationToken);

            return CallResult(comment);
        }

        /// <inheritdoc cref="IForecastService" />
        public async Task<CallResult<ForecastCommentDto?>> GetComment(Guid forecastId, Guid commentId, CancellationToken cancellationToken)
        {
            var comment = await ForecastServiceLogic.GetComment(forecastId, commentId, cancellationToken);

            return CallResult(comment);
        }

        /// <inheritdoc cref="IForecastService" />
        public async Task<CallResult<ForecastCommentDto?>> UpdateComment(Guid forecastId, Guid commentId, ForecastCommentDataDto commentData, CancellationToken cancellationToken)
        {
            if (commentData is null)
            {
                return BadRequest<ForecastCommentDto?>($"{nameof(commentData)} is null");
            }

            var blCommentData = DtoBlMapper.MapToBl(commentData);
            if (blCommentData is null)
            {
                return BadRequest<ForecastCommentDto?>($"{nameof(commentData)} is invalid");
            }

            var comment = await ForecastServiceLogic.UpdateComment(forecastId, commentId, blCommentData, cancellationToken);

            return CallResult(comment);
        }

        /// <inheritdoc />
        public async Task<CallResult<IReadOnlyCollection<WeatherForecastDto>?>> GetLocationForecasts(string location, CancellationToken cancellationToken)
        {
            var forecasts = await ForecastServiceLogic.GetLocationForecasts(location, cancellationToken);

            return CallResult(forecasts);
        }

        private CallResult<IReadOnlyCollection<ForecastCommentDto>?> CallResult(IReadOnlyCollection<ForecastComment>? comments) => CallResult(comments, DtoBlMapper.MapToDto);

        private CallResult<ForecastCommentDto?> CallResult(ForecastComment? comment) => CallResult(comment, DtoBlMapper.MapToDto);

        private CallResult<IReadOnlyCollection<WeatherForecastDto>?> CallResult(IReadOnlyCollection<WeatherForecast>? forecasts) => CallResult(forecasts, DtoBlMapper.MapToDto);

        private CallResult<WeatherForecastDto?> CallResult(WeatherForecast? forecast) => CallResult(forecast, DtoBlMapper.MapToDto);
    }
}
