using System;

namespace ForecastService.Services.BusinessLogic.Model
{
    public sealed class ForecastComment
    {
        public ForecastComment(Guid id, Guid forecastId, CommentData comment)
        {
            Id = id;
            ForecastId = forecastId;
            Comment = comment;
        }

        public Guid Id { get; }

        public Guid ForecastId { get; }

        public CommentData Comment { get; }
    }
}
