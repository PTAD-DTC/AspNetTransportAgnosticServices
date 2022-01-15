using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Client.Base
{
    [PublicAPI]
    public interface ICommunicationDebugListener
    {
        Task OnRequestSend(string? method, string? url, IReadOnlyCollection<string> additionalInfo, string data, CancellationToken cancellationToken);
        Task OnResponseReceived(string? method, string? url, IReadOnlyCollection<string> additionalInfo, string data, CancellationToken cancellationToken);
    }
}
