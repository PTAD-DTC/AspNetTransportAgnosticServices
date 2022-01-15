using CommentService.Interface;

namespace CommentService.RestServer.Controllers.V1_0
{
    /// <summary>
    /// Base class for WeatherService controllers 
    /// </summary>
    public abstract class ControllerBaseCommentService : ControllerBaseCore<ICommentService>
    {
        /// <summary>
        /// ctor
        /// </summary>
        protected ControllerBaseCommentService(ICommentService commentService) : base(commentService, ServiceInfo.ApiVersion)
        {
        }
    }
}
