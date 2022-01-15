using System;
using JetBrains.Annotations;
using Service.Interface.Base;

namespace ForecastService.Interface.Models.DTO
{
    /// <summary>
    /// User Comment for forecast
    /// </summary>
    [PublicAPI]
    public sealed class ForecastCommentDto : ModelBase
    {
        /// <summary>
        /// Serialization ctor
        /// </summary>
        private ForecastCommentDto()
        {

        }

        /// <summary>
        /// ctor
        /// </summary>
        public ForecastCommentDto(Guid id, Guid forecastId, ForecastCommentDataDto commentData)
        {
            Id = id;
            ForecastId = forecastId;
            CommentData = commentData ?? throw new ArgumentNullException(nameof(commentData));
        }

        /// <summary>
        /// Comment id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Forecast id
        /// </summary>
        public Guid ForecastId { get; set; }

        /// <summary>
        /// Comment data
        /// </summary>
        public ForecastCommentDataDto? CommentData { get; set; }
    }
}
