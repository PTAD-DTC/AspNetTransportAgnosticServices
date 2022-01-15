using Client.Runner.Base;
using ForecastClient.Application.Logic.Base;
using ForecastService.BusinessLogic.Implementation;
using ForecastService.CommentService;
using ForecastService.Persistence;
using ForecastService.Persistence.Repository;
using System;
using System.Collections.Immutable;
using ForecastClient.Native.HttpClient;
using ForecastClient.Runner.Native;
using ForecastService.Core;
using ForecastService.DtoBlMapper;

namespace ForecastClient.Application.Logic.NativeDirect
{
    public sealed class ClientApplicationLogicNativeDirect : ClientApplicationLogicBase
    {
        public ClientApplicationLogicNativeDirect(IClientRunnerUserInterface userInterface, string appTitle) : base(userInterface, appTitle)
        {
        }

        private static readonly (ConsoleKey key, string name, Func<IClientRunnerUserInterface, IClientRunner> runnerFactory)[] ExtendedRunners =
        {
            (ConsoleKey.C, "Native Http Client 2.1", userInterface =>
                new ClientRunnerNative(userInterface, true,
                    (info, _) =>
                        new ForecastApiClient(info.serviceUri))),

            (ConsoleKey.F, "Native Rest Client 2.1", userInterface =>
                new ClientRunnerNative(userInterface, true,
                    (info, _) =>
                        new Native.RestClient.ForecastApiClient(info.serviceUri))),

            (ConsoleKey.L, "Service Core 2.1", userInterface =>
                new ClientRunnerNative(userInterface, false,
                    (info, _) =>
                        new ForecastServiceCore(
                            new ForecastServiceLogic(
                                new ForecastDataStorage(
                                    new ForecastMemoryRepository()),
                                new ForecastCommentService(
                                    new CommentServiceFactory())),
                            new DtoBlMapper()))),
        };

        private static readonly Lazy<IImmutableDictionary<ConsoleKey, (string name, Func<IClientRunnerUserInterface, IClientRunner> runnerFactory)>> RunnersInfo =
            new(() => ExtendedRunners.ToImmutableDictionary(
                r => r.key, r => (r.name, r.runnerFactory)));

        protected override IImmutableDictionary<ConsoleKey, (string name, Func<IClientRunnerUserInterface, IClientRunner> runnerFactory)> GetRunnersInfo()
        {
            return RunnersInfo.Value;
        }
    }
}
