using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Runner.Base;

namespace Client.Console.UserInterface
{
    public sealed class ConsoleUserInterface : IClientRunnerUserInterface
    {
        private const ConsoleColor ConsoleBackgroundColor = ConsoleColor.Black;
        private const ConsoleColor ConsoleForegroundColor = ConsoleColor.Gray;
        private const ConsoleColor ConsoleHighlightColor = ConsoleColor.DarkGreen;
        private const ConsoleColor ConsoleWarningColor = ConsoleColor.Magenta;
        private const ConsoleColor ConsoleErrorColor = ConsoleColor.Red;

        private static int HeaderWidth => System.Console.WindowWidth - 20;
        private const char HeaderChar = '-';
        private const char IndentChar = ' ';
        private const int IndentChange = 2;

        private const string ChoiceWithoutKeyReplacement = "--------";
        private readonly StringBuilder _info = new StringBuilder();
        private string? _currentIndent;
        private string? _currentHeader;
        private int _indent;

        public ConsoleUserInterface()
        {
            System.Console.BackgroundColor = ConsoleBackgroundColor;
            System.Console.ForegroundColor = ConsoleForegroundColor;
            UpdateIndent(0);
        }

        private static string CalculateHeader(int indent)
        {
            return new string(HeaderChar, Math.Max(HeaderWidth - indent, 10));
        }

        private void Flush()
        {
            var copy = _info.ToString();
            _info.Clear();
            System.Console.Write(copy);
        }

        private void WithForegroundColor(ConsoleColor foregroundColor, Action action)
        {
            var currentColor = System.Console.ForegroundColor;
            Flush();
            System.Console.ForegroundColor = foregroundColor;
            action();
            Flush();
            System.Console.ForegroundColor = currentColor;
        }

        private void AppendValue(string? info, bool flush = false)
        {
            var fixedLines = info?.Replace($"{Environment.NewLine}", $"{Environment.NewLine}{_currentIndent}");
            _info.Append(fixedLines);

            if (flush)
            {
                Flush();
            }
        }

        private void AppendLine(string? line = null, bool flush = false)
        {
            if (_indent > 0)
            {
                _info.Append(_currentIndent);
            }

            var fixedLines = line?.Replace($"{Environment.NewLine}", $"{Environment.NewLine}{_currentIndent}");
            _info.AppendLine(fixedLines);

            if (flush)
            {
                Flush();
            }
        }

        private void UpdateIndent(int newIndent)
        {
            _indent = Math.Max(newIndent, 0);
            _currentIndent = new string(IndentChar, _indent);
            _currentHeader = CalculateHeader(_indent);
        }
        private void IncreaseIndent()
        {
            UpdateIndent(_indent + IndentChange);
        }
        private void DecreaseIndent()
        {
            UpdateIndent(_indent - IndentChange);
        }

        private ConsoleKey GetUserChoice(string? enterCommandInfo)
        {
            AppendLine();
            AppendValue(_currentIndent);
            AppendValue(enterCommandInfo);
            AppendValue("  ", true);
            var key = System.Console.ReadKey();
            AppendLine(null, true);
            return key.Key;
        }

        public Task SetTitle(string title, CancellationToken cancellationToken)
        {
            System.Console.Title = title;

            return Task.CompletedTask;
        }

        public Task BeginOperation(string? operation, CancellationToken cancellationToken)
        {
            WithForegroundColor(
                ConsoleHighlightColor,
                () =>
                {
                    AppendLine(_currentHeader);
                    IncreaseIndent();
                    AppendLine(operation);
                    DecreaseIndent();
                    AppendLine(_currentHeader, true);
                    IncreaseIndent();
                });

            return Task.CompletedTask;
        }

        public Task EndOperation(string? operation, CancellationToken cancellationToken)
        {
            WithForegroundColor(
                ConsoleHighlightColor,
                () =>
                {
                    if (!string.IsNullOrEmpty(operation))
                    {
                        AppendLine();
                    }
                    AppendLine(operation);
                    DecreaseIndent();
                    AppendLine(_currentHeader);
                    AppendLine();
                });

            return Task.CompletedTask;
        }

        public void AppendInfoLine(string? info)
        {
            AppendLine(info);
        }

        public Task ShowInfo(string? lastLine, CancellationToken cancellationToken)
        {
            AppendLine(lastLine, true);

            return Task.CompletedTask;
        }

        public Task ShowHighlighted(string info, CancellationToken cancellationToken)
        {
            WithForegroundColor(
                ConsoleHighlightColor,
                () =>
                {
                    AppendLine();
                    AppendLine(info);
                    AppendLine();
                });

            return Task.CompletedTask;
        }

        public Task ShowWarning(string warning, CancellationToken cancellationToken)
        {
            WithForegroundColor(
                ConsoleWarningColor,
                () =>
                {
                    AppendLine();
                    AppendLine(warning);
                    AppendLine();
                });

            return Task.CompletedTask;
        }

        public Task ShowError(string error, CancellationToken cancellationToken)
        {
            WithForegroundColor(
                ConsoleErrorColor,
                () =>
                {
                    AppendLine();
                    AppendLine(error);
                    AppendLine();
                });

            return Task.CompletedTask;
        }

        public async Task<ConsoleKey> GetUserChoice(string? header, string? enterCommandInfo, string wrongCommandWarning, IImmutableList<(ConsoleKey? key, string? name)> choices, CancellationToken cancellationToken)
        {
            var set = choices.Select(c => c.key).ToHashSet();

            ConsoleKey? result = null;
            while (!result.HasValue)
            {
                WithForegroundColor(
                    ConsoleHighlightColor,
                    () =>
                    {
                        AppendLine(_currentHeader);
                        IncreaseIndent();
                        AppendLine(header);
                        DecreaseIndent();
                        AppendLine(_currentHeader, true);
                    });

                IncreaseIndent();
                foreach (var (key, name) in choices)
                {
                    var keyInfo = key.HasValue ? $"{key}: {name}" : ChoiceWithoutKeyReplacement;
                    AppendLine(keyInfo);
                }
                AppendLine(null, true);

                var userCommand = GetUserChoice(enterCommandInfo);
                if (set.Contains(userCommand))
                {
                    result = userCommand;
                }
                else
                {
                    AppendLine();
                    await ShowWarning($"{wrongCommandWarning} '{userCommand}'", cancellationToken);
                }
                DecreaseIndent();
            }

            return result.Value;
        }

        public Task ConfirmUserChoice(string? info, CancellationToken cancellationToken)
        {
            AppendLine();
            WithForegroundColor(
                ConsoleHighlightColor,
                () =>
                {
                    IncreaseIndent();
                    AppendLine(info);
                    DecreaseIndent();
                    AppendLine(_currentHeader);
                    AppendLine();
                });

            return Task.CompletedTask;
        }

        public Task<string?> GetUserInfo(string info, CancellationToken cancellationToken)
        {
            WithForegroundColor(
                ConsoleHighlightColor,
                () =>
                {
                    AppendLine(_currentHeader);
                    AppendLine(info);
                    AppendLine(_currentHeader, true);
                });
            AppendValue(_currentIndent);
            Flush();
            var userInfo = System.Console.ReadLine();
            AppendLine(null, true);
            return Task.FromResult(userInfo);
        }
    }
}
