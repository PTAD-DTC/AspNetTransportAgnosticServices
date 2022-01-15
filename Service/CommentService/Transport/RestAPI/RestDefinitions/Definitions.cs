using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.RestDefinitions
{
    public static class Definitions
    {
        public static readonly PathString ServicePathBase = "/api/commentservice";
        public const string ServiceName = "Comment Service";
#if DEBUG
        public static readonly ApiVersion? DefaultApiVersion = new ApiVersion(1, 0);
#else
        public static readonly ApiVersion? DefaultApiVersion = null;
#endif
    }
}
