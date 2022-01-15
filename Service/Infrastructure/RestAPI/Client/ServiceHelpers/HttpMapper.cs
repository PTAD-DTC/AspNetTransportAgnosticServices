using System.Net;
using Service.Interface.Base;

namespace ServiceHelpers
{
    public static class HttpMapper
    {
        public static ResultCode MapToResultCode(this HttpStatusCode statusCode)
        {

            return statusCode switch
            {
                HttpStatusCode.OK => ResultCode.Ok,
                HttpStatusCode.NotFound => ResultCode.NotFound,
                HttpStatusCode.BadRequest => ResultCode.BadRequest,
                var x when (int)x >= 200 && (int)x < 300 => ResultCode.Ok,
                _ => ResultCode.Error
            };
        }

    }
}
