namespace CommentService.Services.BusinessLogic.Model
{
    public sealed class CommentData
    {
        public CommentData(string comment)
        {
            Comment = comment;
        }

        public string Comment { get; }
    }
}
