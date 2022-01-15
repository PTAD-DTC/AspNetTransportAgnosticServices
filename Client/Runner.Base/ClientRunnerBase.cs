using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NewtJson = Newtonsoft.Json;
using SystJson = System.Text.Json;

namespace Client.Runner.Base
{
    public abstract class ClientRunnerBase : IClientRunner
    { 
        protected enum SerializerType
        {
            SystemJson,
            NewtonSoft
        }

        private const SerializerType _defaultSerializerType = SerializerType.SystemJson;
        private readonly SystJson.JsonSerializerOptions _jsonSerializerOptions;
        private readonly NewtJson.JsonSerializerSettings _jsonSerializerSettings;
        private string? _apiVersion;
        private int _getServiceBaseUrlResultIfSelectForbidden;
        private int _selectApiVersionResultIfSelectForbidden;
        private Uri? _serviceBaseUrl;
        private const string ApiVersionV1S0 = "1.0";
        private const string ApiVersionV2S0 = "2.0";
        private const string ApiVersionV2S1 = "2.1";
        private const string ApiVersionV3S0 = "3.0Beta";
        private const string ApiVersionUnspecified = "unspecified";

        private static readonly string[] ValidUriSchemes = new[] { Uri.UriSchemeHttp, Uri.UriSchemeHttps };
        private static readonly Uri DefaultServerHttpsUrl = new Uri("https://localhost:6001/");
        private static readonly Uri DefaultServerUrl = new Uri("http://localhost:6002/");
        private static readonly Uri DefaultFiddlerUrl = new Uri("http://localhost.fiddler:6002/");

        public static readonly (ConsoleKey? key, string? name) ExitOperation = (ConsoleKey.Q, "Exit");

        protected ClientRunnerBase(IClientRunnerUserInterface userInterface)
        {
            UserInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
            _jsonSerializerOptions = new SystJson.JsonSerializerOptions()
            {
                PropertyNamingPolicy = SystJson.JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
            _jsonSerializerOptions.Converters.Add(new SystJson.Serialization.JsonStringEnumConverter(_jsonSerializerOptions.PropertyNamingPolicy));

            _jsonSerializerSettings = new NewtJson.JsonSerializerSettings()
            {
                Formatting = NewtJson.Formatting.Indented
            };
            _jsonSerializerSettings.Converters.Add(new NewtJson.Converters.StringEnumConverter());
        }

        public async Task Run(string? serverAddress, CancellationToken cancellationToken)
        {
            while (await GetServiceBaseUrl(serverAddress, cancellationToken))
            {
                while (await SelectApiVersion(cancellationToken))
                {
                    await DoPrepareClient(cancellationToken);

                    await DoRun(cancellationToken);
                }
            }
            await UserInterface.ShowHighlighted("Stopping runner...", cancellationToken);
        }

        private static IEnumerable<(ConsoleKey? key, string? name)> GetUrlsChoices(Uri? optionalUrl)
        {
            yield return (ConsoleKey.A, DefaultServerHttpsUrl.ToString());
            yield return (ConsoleKey.B, DefaultServerUrl.ToString());
            yield return (ConsoleKey.C, DefaultFiddlerUrl.ToString());
            if (optionalUrl != null)
            {
                yield return (ConsoleKey.D, optionalUrl.ToString());
            }
            yield return (ConsoleKey.E, "Enter server address");

            yield return (null, null);
            yield return ExitOperation;
        }

        private async Task<bool> GetServiceBaseUrl(string? optionalUriString, CancellationToken cancellationToken)
        {
            if (!CanSelectServiceUrl)
            {
                return Interlocked.CompareExchange(ref _getServiceBaseUrlResultIfSelectForbidden, default(int) + 1, default) == default;
            }

            _ = GetUriSafe(optionalUriString, out var optionalUrl);
            var urlChoices = GetUrlsChoices(optionalUrl).ToImmutableArray();
            ConsoleKey? userChoice = null;
            var y = (userChoiceValid: false, doRun: true, url: (Uri?)null);
            while (!y.userChoiceValid)
            {
                userChoice = await UserInterface.GetUserChoice("Select server address", "Command:", "No option", urlChoices, cancellationToken);
                y = userChoice switch
                {
                    ConsoleKey.A => (true, true, DefaultServerHttpsUrl),
                    ConsoleKey.B => (true, true, DefaultServerUrl),
                    ConsoleKey.C => (true, true, DefaultFiddlerUrl),
                    ConsoleKey.D => (optionalUrl != null, true, optionalUrl),
                    ConsoleKey.E => await GetServiceUrl(cancellationToken),
                    var v when v == ExitOperation.key => (true, false, null),
                    _ => (false, false, null)
                };
            }

            if (y.url != null)
            {
                ServiceBaseUrl = y.url;
                await UserInterface.ConfirmUserChoice($"Selected server address: {ServiceBaseUrl}", cancellationToken);
            }
            else
            {
                var (_, name) = urlChoices.FirstOrDefault(o => o.key == userChoice);
                await UserInterface.ConfirmUserChoice(name, cancellationToken);
            }

            return y.doRun;
        }

        private async Task<(bool userChoiceValid, bool doRun, Uri? uri)> GetServiceUrl(CancellationToken cancellationToken)
        {
            var serverUriString = await UserInterface.GetUserInfo("Enter server address: ", cancellationToken);
            var isValid = GetUriSafe(serverUriString, out var uri);
            if (!isValid)
            {
                await UserInterface.ShowWarning("Invalid server address", cancellationToken);
            }
            return (isValid, isValid, uri);
        }

        private static bool GetUriSafe(string? uriString, out Uri? uri)
        {
            uri = null;
            if (Uri.TryCreate(uriString, UriKind.Absolute, out var parsedUri) && ValidUriSchemes.Any(s => string.Equals(parsedUri.Scheme, s)))
            {
                uri = parsedUri;
            }
            return uri != null;
        }

        private static IEnumerable<(ConsoleKey? key, string? name)> GetApiVersionChoices()
        {
            yield return (ConsoleKey.A, ApiVersionV1S0);
            yield return (ConsoleKey.B, ApiVersionV2S0);
            yield return (ConsoleKey.C, ApiVersionV2S1);
            yield return (ConsoleKey.D, ApiVersionV3S0);
            yield return (ConsoleKey.S, ApiVersionUnspecified);

            yield return (null, null);
            yield return ExitOperation;
        }

        private async Task<bool> SelectApiVersion(CancellationToken cancellationToken)
        {
            _apiVersion = null;

            if (!CanSelectApiVersion)
            {
                return Interlocked.CompareExchange(ref _selectApiVersionResultIfSelectForbidden, default(int) + 1, default) == default;
            }

            var apiChoices = GetApiVersionChoices().ToImmutableArray();
            ConsoleKey? userChoice = null;
            var y = (apiSelected: false, doRun: true, apiVersion: (string?)null);
            while (!y.apiSelected)
            {
                userChoice = await UserInterface.GetUserChoice("Select api version:", "Command:", "No api version", apiChoices, cancellationToken);
                y = userChoice switch
                {
                    ConsoleKey.S => (true, true, null),
                    ConsoleKey.A => (true, true, ApiVersionV1S0),
                    ConsoleKey.B => (true, true, ApiVersionV2S0),
                    ConsoleKey.C => (true, true, ApiVersionV2S1),
                    ConsoleKey.D => (true, true, ApiVersionV3S0),
                    var v when v == ExitOperation.key => (true, false, null),
                    _ => (false, false, null)
                };
            }
            if (y.doRun)
            {
                _apiVersion = y.apiVersion;
                await UserInterface.ConfirmUserChoice($"Selected Api Version: {_apiVersion ?? ApiVersionUnspecified}", cancellationToken);
            }
            else
            {
                var (_, name) = apiChoices.FirstOrDefault(o => o.key == userChoice);
                if (!string.IsNullOrEmpty(name))
                {
                    await UserInterface.ConfirmUserChoice(name, cancellationToken);
                }
            }
            return y.doRun;
        }

        private IImmutableList<(ConsoleKey? key, string? name, Func<CancellationToken, Task<bool>> operation)> GetCurrentOperationsInfo()
        {
            var operations = GetCurrentOperationChoices()
                .Concat(new (ConsoleKey? key, string? name, Func<CancellationToken, Task<bool>> operation)[]
                {
                    (null, null, ct => Task.FromResult(false)),
                    (ExitOperation.key, ExitOperation.name, ct => Task.FromResult(false))
                });

            return operations.ToImmutableArray();
        }

        private async Task DoRun(CancellationToken cancellationToken)
        {
            var run = true;
            while (run)
            {
                try
                {
                    var currentOperations = GetCurrentOperationsInfo();
                    var choices = currentOperations.Select(c => (c.key, c.name)).ToImmutableArray();

                    var userChoice = await UserInterface.GetUserChoice("Operations:", "Command: ", "No operation",
                        choices, cancellationToken);

                    var (_, name, operation) = currentOperations.FirstOrDefault(o => o.key == userChoice);
                    if (operation is null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        await UserInterface.ConfirmUserChoice($"Operation: {name}", cancellationToken);
                    }

                    run = await operation(cancellationToken);
                }
                catch (Exception e)
                {
                    await UserInterface.ShowError($"{e.GetType().Name}: {e.Message}", cancellationToken);
                }
            }
        }

        private string Serialize<TValue>(TValue value, SerializerType serializerType = _defaultSerializerType)
        {
            return serializerType switch
            {
                SerializerType.SystemJson => SystJson.JsonSerializer.Serialize(value, _jsonSerializerOptions),
                SerializerType.NewtonSoft => NewtJson.JsonConvert.SerializeObject(value, _jsonSerializerSettings),
                _ => SystJson.JsonSerializer.Serialize(value, _jsonSerializerOptions),
            };
        }

        protected async Task DoOperation(string? operationHeader, Func<CancellationToken, Task> operation, CancellationToken cancellationToken)
        {
            await UserInterface.BeginOperation(operationHeader, cancellationToken);
            try
            {
                await operation(cancellationToken);
            }
            catch (Exception e)
            {
                await UserInterface.ShowError($"{e.GetType().Name}: {e.Message}", cancellationToken);
            }
            finally
            {
                await UserInterface.EndOperation(operationHeader, cancellationToken);
            }
        }

        public async Task OnRequestSend(string? method, string? url, IReadOnlyCollection<string> additionalInfo, string? data, CancellationToken cancellationToken)
        {
            await DoOperation("Request",
                _ =>
                {
                    UserInterface.AppendInfoLine($"{method} {url}");
                    foreach (var info in additionalInfo)
                    {
                        UserInterface.AppendInfoLine(info);
                    }
                    if (!string.IsNullOrEmpty(data))
                    {
                        UserInterface.AppendInfoLine($"{data}");
                    }

                    return Task.CompletedTask;
                },
                cancellationToken);
        }

        public async Task OnResponseReceived(string? method, string? url, IReadOnlyCollection<string> additionalInfo, string? data, CancellationToken cancellationToken)
        {
            await DoOperation("Response",
                _ =>
                {
                    UserInterface.AppendInfoLine($"{method} {url}");
                    foreach (var info in additionalInfo)
                    {
                        UserInterface.AppendInfoLine(info);
                    }
                    if (!string.IsNullOrEmpty(data))
                    {
                        UserInterface.AppendInfoLine($"{data}");
                    }

                    return Task.CompletedTask;
                },
                cancellationToken);
        }

        protected string GetApiVersionInfo()
        {
            return $"apiVersion: {ApiVersion ?? ApiVersionUnspecified}";
        }

        protected async Task ShowSerializedData<TValue>(TValue data, CancellationToken cancellationToken, SerializerType serializerType = _defaultSerializerType)
        {
            await DoOperation("Serialized data",
                _ =>
                {
                    UserInterface.AppendInfoLine(Serialize(data, serializerType));

                    return Task.CompletedTask;
                },
                cancellationToken);
        }

        protected IClientRunnerUserInterface UserInterface { get; }

        protected virtual bool CanSelectServiceUrl => true;

        protected virtual bool CanSelectApiVersion => true;

        protected Uri ServiceBaseUrl
        {
            get => _serviceBaseUrl ?? DefaultServerUrl;
            private set => _serviceBaseUrl = value;
        }

        protected virtual string? ApiVersion => _apiVersion;

        protected abstract IEnumerable<(ConsoleKey? key, string? name, Func<CancellationToken, Task<bool>> operation)> GetCurrentOperationChoices();

        protected abstract Task DoPrepareClient(CancellationToken cancellationToken);

        protected abstract void ClearData();
    }

    public abstract class ClientRunnerBase<TServiceClient> : ClientRunnerBase
    {
        protected ClientRunnerBase(IClientRunnerUserInterface userInterface) : base(userInterface)
        {
            Client = default!;
        }

        protected override async Task DoPrepareClient(CancellationToken cancellationToken)
        {
            await DoOperation("Runner init",
                async ct =>
                {
                    UserInterface.AppendInfoLine($"Starting {GetType().Name}<{typeof(TServiceClient).Name}> {GetApiVersionInfo()}");

                    Client = default!;
                    Client = PrepareClient(ct);

                    if (Client is null)
                    {
                        await UserInterface.ShowWarning("Client not created", ct);
                    }
                    UserInterface.AppendInfoLine($"{GetType().Name}<{Client?.GetType().Name}> {GetApiVersionInfo()} started");
                    UserInterface.AppendInfoLine("  Client type:");
                    var clientType = Client?.GetType();
                    while (clientType is { })
                    {
                        UserInterface.AppendInfoLine($"    {clientType.FullName}");
                        clientType = clientType.BaseType;
                    }

                    UserInterface.AppendInfoLine($"Client runner {(Client is null ? "NOT " : null)}ready");
                },
                cancellationToken);
        }

        protected TServiceClient Client { get; private set; }

        protected abstract TServiceClient PrepareClient(CancellationToken cancellationToken);
    }
}
