// See https://aka.ms/new-console-template for more information
using ForecastClient.Native.gRPC;

// The port number must match the port of the gRPC server.
using var client = new ForecastGrpcClient(new Uri("https://localhost:7060"));
var reply = await client.GetTodayForecasts(CancellationToken.None);
Console.WriteLine($"Found {reply.Result!.Count} forecasts");
foreach (var w in reply.Result!)
{
    Console.WriteLine($"  {w.Location} [{w.Probability}]: {w.TemperatureC} - {w.Description}");
}
Console.WriteLine("");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
