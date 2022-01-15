using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Runner.Base
{
    public interface IClientRunnerUserInterface
    {
        Task SetTitle(string title, CancellationToken cancellationToken);
        Task BeginOperation(string? operationInfo, CancellationToken cancellationToken);
        Task EndOperation(string? operationInfo, CancellationToken cancellationToken);

        void AppendInfoLine(string? info);
        Task ShowInfo(string lastLine, CancellationToken cancellationToken);
        Task ShowHighlighted(string info, CancellationToken cancellationToken);
        Task ShowWarning(string warning, CancellationToken cancellationToken);
        Task ShowError(string error, CancellationToken cancellationToken);

        Task<ConsoleKey> GetUserChoice(string? header, string? enterCommandInfo, string wrongCommandWarning, IImmutableList<(ConsoleKey? key, string? name)> choices, CancellationToken cancellationToken);
        Task ConfirmUserChoice(string? info, CancellationToken cancellationToken);
        Task<string?> GetUserInfo(string info, CancellationToken cancellationToken);
    }
}
