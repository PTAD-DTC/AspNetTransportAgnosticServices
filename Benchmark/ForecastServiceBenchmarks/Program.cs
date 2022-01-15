
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using ForecastServiceBenchmarks;

Console.WriteLine("-------------------------------------------------------");
Console.WriteLine("               Starting benchmark ...");
Console.WriteLine("-------------------------------------------------------");
Console.WriteLine();

BenchmarkRunner.Run<GetTodayForecastBenchmark>(
    ManualConfig
        .Create(DefaultConfig.Instance)
        .WithOptions(ConfigOptions.JoinSummary | ConfigOptions.DisableLogFile));

Console.WriteLine();
Console.WriteLine("-------------------------------------------------------");
Console.WriteLine("               Benchmark completed");
Console.WriteLine("-------------------------------------------------------");
