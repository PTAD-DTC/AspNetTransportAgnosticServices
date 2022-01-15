using JetBrains.Annotations;

namespace Service.Interface.Base
{
    [PublicAPI]
    public interface IServiceBase
    {
        string? ApiVersion { get; }
    }
}
