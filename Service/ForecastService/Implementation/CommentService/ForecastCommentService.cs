using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommentService.Interface;
using ForecastService.Services.BusinessLogic.Model;
using ForecastService.Services.CommentService;

namespace ForecastService.CommentService
{
    public sealed class ForecastCommentService : IForecastCommentService
    {
        private readonly ICommentServiceFactory _commentServiceFactory;

        public ForecastCommentService(ICommentServiceFactory commentServiceFactory)
        {
            _commentServiceFactory = commentServiceFactory ?? throw new ArgumentNullException(nameof(commentServiceFactory));
        }

        private async Task<IReadOnlyCollection<ForecastComment>?> getComments(Guid forecastId, CancellationToken cancellationToken)
        {
            var comment = await CallCommentService(
                (cs, ct) => cs.GetSubjectComments(forecastId, ct),
                cancellationToken);

            return comment.Result.MapSafe();
        }

        private async Task SeedComments(Guid forecastId, CancellationToken cancellationToken)
        {
            foreach (var i in Enumerable.Range(0, 3))
            {
                await AddComment(forecastId, new CommentData($"empty comment #{i}"), cancellationToken);
            }
        }

        private async Task<T> CallCommentService<T>(Func<ICommentService, CancellationToken, Task<T>> serviceFunc, CancellationToken cancellationToken)
        {
            Debug.Assert(serviceFunc != null, $"{nameof(serviceFunc)} is null");

            var commentService = _commentServiceFactory.GetCommentService();
            var res = await serviceFunc(commentService, cancellationToken);
            return res;
        }

        /// <inheritdoc />
        public async Task<ForecastComment?> AddComment(Guid forecastId, CommentData commentData, CancellationToken cancellationToken)
        {
            var comment = await CallCommentService(
                (cs, ct) => cs.AddComment(forecastId, commentData.Map(), ct),
                cancellationToken);
            return comment.Result.MapSafe();
        }

        /// <inheritdoc />
        public async Task<ForecastComment?> GetComment(Guid id, CancellationToken cancellationToken)
        {
            var comment = await CallCommentService(
                (cs, ct) => cs.FindComment(id, ct),
                cancellationToken);
            return comment.Result.MapSafe();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<ForecastComment>?> GetComments(Guid forecastId, CancellationToken cancellationToken)
        {
            var comments = await getComments(forecastId, cancellationToken);

            if (comments is null || comments.Count == 0)
            {
                await SeedComments(forecastId, cancellationToken);
                comments = await getComments(forecastId, cancellationToken);
            }

            return comments;
        }

        /// <inheritdoc />
        public async Task<ForecastComment?> UpdateComment(Guid id, CommentData commentData, CancellationToken cancellationToken)
        {
            var comment = await CallCommentService(
                (cs, ct) => cs.UpdateComment(id, commentData.Map(), ct),
                cancellationToken);
            return comment.Result.MapSafe();
        }
    }
}
