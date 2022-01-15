using System.Collections.Generic;
using CommentService.Interface.Models.DTO;
using CommentService.Services.BusinessLogic.Model;

namespace CommentService.Core
{
    public interface IDtoBlMapper
    {
        CommentDto? MapToDto(Comment? forecast);

        IEnumerable<CommentDto> MapToDto(IEnumerable<Comment> data);

        CommentData? MapToBl(CommentDataDto? commentData);
    }
}