using System.Net;
using Microsoft.AspNetCore.Mvc;
#if USESWAGGER
using Swashbuckle.AspNetCore.Annotations;
#endif

namespace Service.RestServer.Base
{
    /// <summary>
    /// Debug controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("0.0")]
    public class DebugController : ControllerBase
    {
        /// <summary>
        /// Returns pong response
        /// </summary>
        [HttpGet("ping")]
#if USESWAGGER
        [SwaggerResponse((int)HttpStatusCode.OK, "Pong is returned", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Request failed due to server-side error")]
#endif
        public ActionResult<string> PingGet()
        {
            return Ok("pong");
        }

        /// <summary>
        /// Returns pong response
        /// </summary>
        [HttpPut("ping")]
#if USESWAGGER
        [SwaggerResponse((int)HttpStatusCode.OK, "Pong is returned", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Request failed due to server-side error")]
#endif
        public ActionResult<string> PingPut()
        {
            return Ok("pong");
        }

        /// <summary>
        /// Returns pong response
        /// </summary>
        [HttpPost("ping")]
#if USESWAGGER
        [SwaggerResponse((int)HttpStatusCode.OK, "Pong is returned", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Request failed due to server-side error")]
#endif
        public ActionResult<string> PingPost()
        {
            return Ok("pong");
        }


        /// <summary>
        /// Returns pong response
        /// </summary>
        [HttpDelete("ping")]
#if USESWAGGER
        [SwaggerResponse((int)HttpStatusCode.OK, "Pong is returned", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Request failed due to server-side error")]
#endif
        public ActionResult<string> PingDelete()
        {
            return Ok("pong");
        }
    }
}
