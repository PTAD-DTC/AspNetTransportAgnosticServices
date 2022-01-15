using System.Collections.Generic;
using CommentService.Interface.Models.DTO;
using CommentService.Services.BusinessLogic.Model;
using Helpers;

namespace CommentService.DtoBlMapper.Extensions
{
    internal static class DtoMapper
    {
        private static CommentDataDto MapToDto(this CommentData commentData)
        {
            return new CommentDataDto(commentData.Comment);
        }

        private static CommentDataDto? MapToDtoSafe(this CommentData? commentData)
        {
            return commentData?.MapToDto();
        }

        private static CommentDto MapToDto(this Comment comment)
        {
            var commentDataDto = comment.CommentData.MapToDtoSafe() ?? new CommentDataDto(string.Empty);
            return new CommentDto(
                    comment.Id,
                    comment.CommentSubjectId,
                    commentDataDto);
        }

        public static CommentDto? MapToDtoSafe(this Comment? comment)
        {
            return (comment?.CommentData is null)
                ? null
                : comment.MapToDto();
        }

        public static IEnumerable<CommentDto> MapToDto(this IEnumerable<Comment> data)
        {
            return data.MapTo(MapToDto)!;
        }
    }
}
