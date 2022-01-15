using System.Threading;
using System.Threading.Tasks;

namespace Client.Runner.Base
{
    public interface IClientRunner
    {
        Task Run(string? serverAddress, CancellationToken cancellationToken);
    }
}
