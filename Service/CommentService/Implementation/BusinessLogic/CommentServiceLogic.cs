using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommentService.Services.BusinessLogic;
using CommentService.Services.BusinessLogic.Model;
using CommentService.Services.Persistence;

namespace CommentService.BusinessLogic
{
    public sealed class CommentServiceLogic : ICommentServiceLogic
    {
        private readonly ICommentDataStorage _dataStorage;

        public CommentServiceLogic(ICommentDataStorage dataStorage)
        {
            _dataStorage = dataStorage ?? throw new ArgumentNullException(nameof(dataStorage));
        }

        public async Task<Comment?> AddComment(Guid subjectId, CommentData commentData, CancellationToken cancellationToken)
        {
            var newComment = await _dataStorage.CommentRepository.AddComment(subjectId, commentData, cancellationToken);
            return newComment;
        }

        public async Task<Comment?> FindComment(Guid commentId, CancellationToken cancellationToken)
        {
            var comment = await _dataStorage.CommentRepository.GetComment(commentId, cancellationToken);
            return comment;
        }

        public async Task<IReadOnlyCollection<Comment>?> GetSubjectComments(Guid subjectId, CancellationToken cancellationToken)
        {
            var data = await _dataStorage.CommentRepository.GetSubjectComments(subjectId, cancellationToken);
            return data;
        }

        public async Task<Comment?> UpdateComment(Guid commentId, CommentData commentData, CancellationToken cancellationToken)
        {
            var updatedComment = await _dataStorage.CommentRepository.UpdateComment(commentId, commentData, cancellationToken);
            return updatedComment;
        }
    }
}
