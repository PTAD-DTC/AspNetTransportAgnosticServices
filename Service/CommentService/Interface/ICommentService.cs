using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommentService.Interface.Models.DTO;
using Service.Interface.Base;

namespace CommentService.Interface
{
    public interface ICommentService
    {
        string ApiVersion { get; }

        /// <summary>
        /// Returns comments for subject
        /// </summary>
        /// <param name="subjectId">Subject id</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<CallResult<IReadOnlyCollection<CommentDto>?>> GetSubjectComments(Guid subjectId, CancellationToken cancellationToken);

        /// <summary>
        /// Returns specified comment
        /// </summary>
        /// <param name="commentId">Comment id</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<CallResult<CommentDto?>> FindComment(Guid commentId, CancellationToken cancellationToken);

        /// <summary>
        /// Creates new comment
        /// </summary>
        /// <param name="subjectId">Subject id</param>
        /// <param name="commentData">Comment request</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<CallResult<CommentDto?>> AddComment(Guid subjectId, CommentDataDto commentData, CancellationToken cancellationToken);

        /// <summary>
        /// Updates comment
        /// </summary>
        /// <param name="commentId">comment id</param>
        /// <param name="commentData">Comment</param>
        /// <param name="cancellationToken">Operation cancellation notification</param>
        Task<CallResult<CommentDto?>> UpdateComment(Guid commentId, CommentDataDto commentData, CancellationToken cancellationToken);
    }
}
