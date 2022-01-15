using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Runner.Base;

namespace ForecastClient.Application.Logic.Base
{
    public abstract class ClientApplicationLogicBase
    {
        private readonly string _appTitle;
        protected ClientApplicationLogicBase(IClientRunnerUserInterface userInterface, string appTitle)
        {
            _appTitle = appTitle;
            UserInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
        }

        private IClientRunnerUserInterface UserInterface { get; }

        public async Task Run(string? serverAddress, CancellationToken cancellationToken)
        {
            (bool exit, IClientRunner? runner) runnerInfo = (false, null);

            await UserInterface.SetTitle(_appTitle, cancellationToken);
            await UserInterface.BeginOperation(_appTitle, cancellationToken);
            UserInterface.AppendInfoLine("Starting application ...");
            await UserInterface.EndOperation(null, cancellationToken);

            while (!runnerInfo.exit)
            {
                runnerInfo = await GetClientRunner(cancellationToken);
                if (runnerInfo.runner is { })
                {
                    await runnerInfo.runner.Run(serverAddress, cancellationToken);
                }
                await UserInterface.SetTitle(_appTitle, cancellationToken);
            }
            await UserInterface.ShowHighlighted("Stopping application ...", cancellationToken);
        }

        private async Task<(bool exit, IClientRunner? runner)> GetClientRunner(CancellationToken cancellationToken)
        {
            var y = (userChoiceValid: false, clientName: (string?)null, clientRunner: (IClientRunner?)null);
            while (!y.userChoiceValid)
            {
                y = await GetClientRunnerAction(cancellationToken);
            }

            if (y.clientRunner is { })
            {
                await UserInterface.SetTitle($"{_appTitle}: {y.clientName}", cancellationToken);
            }

            return (y.userChoiceValid && y.clientRunner is null, y.clientRunner);
        }

        private async Task<(bool userChoiceValid, string? clientName, IClientRunner? clientRunner)> GetClientRunnerAction(CancellationToken cancellationToken)
        {
            var runnersInfo = GetRunnersInfo();

            var query = runnersInfo.Select(r => ((ConsoleKey?)r.Key, r.Value.name))
                .Concat(new (ConsoleKey? Key, string? name)[]
                {
                    (null, null),
                    ClientRunnerBase.ExitOperation
                });
            var choices = query.ToImmutableArray();

            var userCommand = await UserInterface.GetUserChoice("Select client type", "Command:", "No client type", choices, cancellationToken);

            if (userCommand == ClientRunnerBase.ExitOperation.key)
            {
                await UserInterface.ConfirmUserChoice(ClientRunnerBase.ExitOperation.name, cancellationToken);
                return (true, null, null);
            }

            if (runnersInfo.TryGetValue(userCommand, out var runnerInfo) &&
                runnerInfo.runnerFactory(UserInterface) is {} runner)
            {
                await UserInterface.ConfirmUserChoice($"Selected client: {runnerInfo.name} ({runner.GetType().Name})", cancellationToken);
                return (true, runnerInfo.name, runner);
            }

            await UserInterface.ShowError($"Unable to start client {runnerInfo.name}", cancellationToken);
            return (false, null, null);
        }

        protected abstract IImmutableDictionary<ConsoleKey, (string? name, Func<IClientRunnerUserInterface, IClientRunner> runnerFactory)> GetRunnersInfo();
    }
}
