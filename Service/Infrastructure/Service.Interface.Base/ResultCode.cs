using JetBrains.Annotations;

namespace Service.Interface.Base
{
    [PublicAPI]
    public enum ResultCode
    {
        Ok,
        NotFound,
        BadRequest,
        Error
    }
}
