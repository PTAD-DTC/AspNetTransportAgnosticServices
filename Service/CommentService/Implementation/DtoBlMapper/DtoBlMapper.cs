using System.Collections.Generic;
using CommentService.Core;
using CommentService.DtoBlMapper.Extensions;
using CommentService.Interface.Models.DTO;
using CommentService.Services.BusinessLogic.Model;

namespace CommentService.DtoBlMapper
{
    public sealed class DtoBlMapper : IDtoBlMapper
    {
        public CommentData? MapToBl(CommentDataDto? commentData)
        {
            return commentData.MapToDtoSafe();
        }

        public CommentDto? MapToDto(Comment? comment)
        {
            return comment.MapToDtoSafe();
        }

        public IEnumerable<CommentDto> MapToDto(IEnumerable<Comment> data)
        {
            return data.MapToDto();
        }
    }
}
