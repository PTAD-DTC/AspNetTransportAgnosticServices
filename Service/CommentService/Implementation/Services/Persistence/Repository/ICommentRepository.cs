using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommentService.Services.BusinessLogic.Model;

namespace CommentService.Services.Persistence.Repository
{
    public interface ICommentRepository
    {
        Task<IReadOnlyCollection<Comment>> GetSubjectComments(Guid subjectId, CancellationToken cancellationToken);

        Task<Comment?> GetComment(Guid commentId, CancellationToken cancellationToken);

        Task<Comment?> AddComment(Guid subjectId, CommentData commentData, CancellationToken cancellationToken);

        Task<Comment?> UpdateComment(Guid commentId, CommentData commentData, CancellationToken cancellationToken);

        Task<Comment?> DeleteComment(Guid commentId, CancellationToken cancellationToken);
    }
}
