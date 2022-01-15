using JetBrains.Annotations;

namespace Service.Interface.Base
{
    [PublicAPI]
    public class CallResult
    {
        public CallResult(ResultCode resultCode, string? apiVersion = null, string? description = null)
        {
            ResultCode = resultCode;
            ApiVersion = apiVersion;
            Description = description;
        }

        public ResultCode ResultCode { get; }
        public string? ApiVersion { get; }
        public string? Description { get; }
    }

    [PublicAPI]
    public class CallResult<TResult> : CallResult
    {
        public CallResult(TResult result) : base(ResultCode.Ok)
        {
            Result = result;
        }

        public CallResult(TResult result, string? apiVersion = null) : base(ResultCode.Ok, apiVersion)
        {
            Result = result;
        }

        public CallResult(ResultCode resultCode, string? apiVersion = null, string? description = null) : base(resultCode, apiVersion, description)
        {
        }

        public TResult Result { get; } = default!;
    }
}
