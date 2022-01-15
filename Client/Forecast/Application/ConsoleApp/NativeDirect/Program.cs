using System;
using System.Threading;
using System.Threading.Tasks;
using Client.Console.UserInterface;
using ForecastClient.Application.Logic.NativeDirect;

namespace ForecastClient.ConsoleApp.NativeDirect
{
    class Program
    {
        private const string AppTitle = "Forecast Client ServiceCore";
        static Program()
        {
            Console.Title = AppTitle;
        }

        static async Task Main(string[] args)
        {
            var runner = new ClientApplicationLogicNativeDirect(new ConsoleUserInterface(), AppTitle);
            await runner.Run(args?.Length > 0 ? args[0] : null, CancellationToken.None);
        }
    }
}
