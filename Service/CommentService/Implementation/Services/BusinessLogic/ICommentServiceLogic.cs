using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommentService.Services.BusinessLogic.Model;

namespace CommentService.Services.BusinessLogic
{
    public interface ICommentServiceLogic
    {
        /// <summary>
        /// Returns comments for subject
        /// </summary>
        /// <param name="subjectId">Subject id</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<IReadOnlyCollection<Comment>?> GetSubjectComments(Guid subjectId, CancellationToken cancellationToken);

        /// <summary>
        /// Creates new comment
        /// </summary>
        /// <param name="subjectId">Subject id</param>
        /// <param name="commentData">Comment data</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<Comment?> AddComment(Guid subjectId, CommentData commentData, CancellationToken cancellationToken);

        /// <summary>
        /// Returns specified comment
        /// </summary>
        /// <param name="commentId">Comment id</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<Comment?> FindComment(Guid commentId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates comment
        /// </summary>
        /// <param name="commentId">comment id</param>
        /// <param name="commentData">Comment</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<Comment?> UpdateComment(Guid commentId, CommentData commentData, CancellationToken cancellationToken);
    }
}