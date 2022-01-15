using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommentService.Interface;
using CommentService.Interface.Models.DTO;
using CommentService.Services.BusinessLogic;
using Service.Interface.Base;

namespace CommentService.Core
{
    public sealed class CommentServiceCore : ServiceCoreBase, ICommentService
    {
        private readonly ICommentServiceLogic _commentServiceLogic;
        private readonly IDtoBlMapper _dtoBlMapper;

        public CommentServiceCore(ICommentServiceLogic commentServiceLogic, IDtoBlMapper dtoBlMapper) : base(ServiceInfo.ApiVersion)
        {
            _commentServiceLogic = commentServiceLogic ?? throw new ArgumentNullException(nameof(commentServiceLogic));
            _dtoBlMapper = dtoBlMapper ?? throw new ArgumentNullException(nameof(dtoBlMapper));
        }

        /// <inheritdoc />
        public async Task<CallResult<CommentDto?>> AddComment(Guid subjectId, CommentDataDto commentData, CancellationToken cancellationToken)
        {
            if (commentData is null)
            {
                return BadRequest<CommentDto?>();
            }

            var blCommentData = _dtoBlMapper.MapToBl(commentData);
            if (blCommentData is null)
            {
                return BadRequest<CommentDto?>();
            }

            var comment = await _commentServiceLogic.AddComment(subjectId, blCommentData, cancellationToken);

            return comment is null ? NotFound<CommentDto?>() : Ok(_dtoBlMapper.MapToDto(comment));
        }

        /// <inheritdoc />
        public async Task<CallResult<CommentDto?>> FindComment(Guid commentId, CancellationToken cancellationToken)
        {
            var comment = await _commentServiceLogic.FindComment(commentId, cancellationToken);

            return comment is null ? NotFound<CommentDto?>() : Ok(_dtoBlMapper.MapToDto(comment));
        }

        /// <inheritdoc />
        public async Task<CallResult<IReadOnlyCollection<CommentDto>?>> GetSubjectComments(Guid subjectId, CancellationToken cancellationToken)
        {
            var comments = await _commentServiceLogic.GetSubjectComments(subjectId, cancellationToken);
            return comments?.Count > 0
                ? Ok<IReadOnlyCollection<CommentDto>?>(_dtoBlMapper.MapToDto(comments).ToArray())
                : NotFound<IReadOnlyCollection<CommentDto>?>();
        }

        /// <inheritdoc />
        public async Task<CallResult<CommentDto?>> UpdateComment(Guid commentId, CommentDataDto commentData, CancellationToken cancellationToken)
        {
            if (commentData is null)
            {
                return BadRequest<CommentDto?>();
            }

            var blCommentData = _dtoBlMapper.MapToBl(commentData);
            if (blCommentData is null)
            {
                return BadRequest<CommentDto?>();
            }

            var comment = await _commentServiceLogic.UpdateComment(commentId, blCommentData, cancellationToken);

            return comment is null ? NotFound<CommentDto?>() : Ok(_dtoBlMapper.MapToDto(comment));
        }
    }
}
