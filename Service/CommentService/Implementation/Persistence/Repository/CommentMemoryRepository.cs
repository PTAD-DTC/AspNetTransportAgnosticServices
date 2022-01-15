using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommentService.Persistence.Repository.Model;
using CommentService.Services.BusinessLogic.Model;
using CommentService.Services.Persistence.Repository;

namespace CommentService.Persistence.Repository
{
    public sealed class CommentMemoryRepository : ICommentRepository
    {
        private static readonly ConcurrentDictionary<Guid, SubjectCommentData> Storage = new ConcurrentDictionary<Guid, SubjectCommentData>();

        public Task<IReadOnlyCollection<Comment>> GetSubjectComments(Guid subjectId, CancellationToken cancellationToken)
        {
            var raw = Storage.ToArray();

            var data = raw
                .Where(f => f.Value.CommentSubjectId == subjectId)
                .Select(f => f.Value.MapToBl())
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<Comment>>(data);
        }

        public Task<Comment?> GetComment(Guid commentId, CancellationToken cancellationToken)
        {
            var success = Storage.TryGetValue(commentId, out var commentData);

            return Task.FromResult(success ? commentData.MapToBlSafe() : null);
        }

        public Task<Comment?> AddComment(Guid subjectId, CommentData commentData, CancellationToken cancellationToken)
        {
            var comment = new SubjectCommentData(Guid.NewGuid(), subjectId, commentData.Comment);
            if (!Storage.TryAdd(comment.Id, comment))
            {
                comment = null;
            }

            return Task.FromResult(comment.MapToBlSafe());
        }

        public Task<Comment?> DeleteComment(Guid commentId, CancellationToken cancellationToken)
        {
            var success = Storage.TryRemove(commentId, out var commentData);

            return Task.FromResult(success ? commentData.MapToBlSafe() : null);
        }

        public Task<Comment?> UpdateComment(Guid commentId, CommentData commentData, CancellationToken cancellationToken)
        {
            var exists = Storage.TryGetValue(commentId, out var actualData);

            if (!exists || actualData is null)
            {
                return Task.FromResult<Comment?>(null);
            }

            var updated = new SubjectCommentData(commentId, actualData.CommentSubjectId, commentData.Comment);
            var success = Storage.TryUpdate(commentId, updated, actualData);
            return Task.FromResult(success ? updated.MapToBlSafe() : null);
        }
    }
}
