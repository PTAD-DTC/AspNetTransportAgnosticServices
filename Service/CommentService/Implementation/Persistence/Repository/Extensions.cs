using CommentService.Persistence.Repository.Model;
using CommentService.Services.BusinessLogic.Model;

namespace CommentService.Persistence.Repository
{
    internal static class Extensions
    {
        public static Comment MapToBl(this SubjectCommentData subjectCommentData)
        {
            return new Comment(
                subjectCommentData.Id,
                subjectCommentData.CommentSubjectId,
                new CommentData(subjectCommentData.Comment));
        }

        public static Comment? MapToBlSafe(this SubjectCommentData? subjectCommentData)
        {
            return subjectCommentData is null ? null : subjectCommentData.MapToBl();
        }
    }
}
