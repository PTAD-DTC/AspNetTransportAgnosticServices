using System;

namespace CommentService.Persistence.Repository.Model
{
    internal sealed class SubjectCommentData
    {
        public SubjectCommentData(Guid id, Guid commentSubjectId, string comment)
        {
            Id = id;
            CommentSubjectId = commentSubjectId;
            Comment = comment;
        }

        public Guid Id { get; }

        public Guid CommentSubjectId { get; }

        public string Comment { get; }
    }
}
