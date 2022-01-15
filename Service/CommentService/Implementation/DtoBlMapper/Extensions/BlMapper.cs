using CommentService.Interface.Models.DTO;
using CommentService.Services.BusinessLogic.Model;

namespace CommentService.DtoBlMapper.Extensions
{
    internal static class BlMapper
    {
        private static CommentData MapToDto(this CommentDataDto commentData)
        {
            return new CommentData(commentData.Comment ?? string.Empty);
        }

        public static CommentData? MapToDtoSafe(this CommentDataDto? commentData)
        {
            return commentData?.Comment is null
                ? null
                : commentData.MapToDto();
        }
    }
}
