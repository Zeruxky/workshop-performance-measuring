# Workshop performance measuring

This workshop was performed on 03/12/2024 internally at World-Direct eBusiness solutions GmbH. The topic of this workshop was the measuring of an applications performance by using `BenchmarkDotnet`. All slides and codes are stored within this repository for self studying.

## What is performance measuring?

> *Performance measurement* is the process of collecting, analyzing and/or reporting information regarding the performance of an individual, group, organization, system or component.[^1]

Performance measuring applies to many different scenarios like measuring the productivity of a factory, the growth of a company's revenue, the up- and downtime of a system or the time and resource consumption of a software. In this workshop we are focusing on the latter part - the software.

There are many indicators for the performance of a software, which can tell you in more detail, how your software performs under certain conditions. Some of this indicators, also called as `key performance indicators (KPIs)`[^2], are for example:

- the average time of an algorithm
- the memory consumption of your application
- the utilization of threads/cores
- the frames per seconds of a video game

You can measure some of these indicators in a simple way by yourself, for example by using a [Stopwatch](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-8.0) or a performance profiler, which is build-in in Visual Studio[^3] or Rider[^4]. Others needs specialized tools to perform the measurement like 3DMark for video games, but we are focusing here only on programs for .NET. These tools are measuring these indicators only when the application is running and you are explicit using them, but to get a meaningful result of your application's indicators you need more than a single measurement sample. To get a bulletproof indicator for your application's performance, you must apply these measurements multiple times under the same conditions and with the same baseline value over and over. This procedure is also called as `benchmarking`. But how you can assure that you provide the same testing conditions for each run? Do you know, how to statistically interpret the results in a meaningful way?

No? Then here comes `BenchmarkDotnet` to the rescue, which takes the burdens, pitfalls and mistakes you are facing during this procedure.

## BenchmarkDotnet

`BenchmarkDotnet` is an open-source tool written by [Andrey Akinshin](https://github.com/AndreyAkinshin) and other community members. Almost all parts of `BenchmarkDotnet` are written in C\# and the project is supported by the .NET Foundation[^5], so indirectly also by Microsoft, and AWS Open Source Software Foundation[^6]. Its widely adopted by many other open-source-projects to display their software performances and its also extensively used by Microsoft itself to express their continuous improvements in .NET, ASP.NET Core and other subparts.

`BenchmarkDotnet` describes itself like following:
> *BenchmarkDotNet* helps you to transform methods into benchmarks, track their performance, and share reproducible measurement experiments. It's no harder than writing unit tests! Under the hood, it performs a lot of [magic](https://benchmarkdotnet.org/#automation) that guarantees [reliable and precise](https://benchmarkdotnet.org/#reliability) results thanks to the [perfolizer](https://github.com/AndreyAkinshin/perfolizer) statistical engine. BenchmarkDotNet protects you from popular benchmarking mistakes and warns you if something is wrong with your benchmark design or obtained measurements. The results are presented in a [user-friendly](https://benchmarkdotnet.org/#friendliness) form that highlights all the important facts about your experiment. BenchmarkDotNet is already adopted by [19100+ GitHub projects](https://github.com/dotnet/BenchmarkDotNet/network/dependents) including [.NET Runtime](https://github.com/dotnet/runtime), [.NET Compiler](https://github.com/dotnet/roslyn), [.NET Performance](https://github.com/dotnet/performance), and many others.

`BenchmarkDotnet` provides a simple API to benchmark .NET code of any size. The configuration of a benchmark is achieved by using one of the many attributes, which let you decide the .NET version to run, several exporters and diagnoser. The usage of `BenchmarkDotnet` will be described in the following chapters.

## How to run a benchmark with BenchmarkDotnet?

All of the following code is located within this repository. The IDE I use for the following examples is `Rider`, but they should also be similar for `Visual Studio` and `Visual Studio Code`. Also the following chapters are based on the documentation of `BenchmarkDotnet`[^7], where you can read more about `BenchmarkDotnet`.

1. First you create a console application, let's call it `Workshop.PerformanceMeasuring.BenchmarkDotnetDemo`, within your IDE.
2. Install the NuGet package for `BenchmarkDotnet` through the IDE's UI or by the following command, which you are executing within your console applications folder (where your `.csproj` is located, so `./Workshop.PerformanceMeasuring/Workshop.PerformanceMeasuring.BenchmarkDotnetDemo`).

  ```bash
  > dotnet add package BenchmarkDotNet
  ```

  ![Installation of BenchmarkDotnet via NuGet within Rider](/assets/images/installation_of_benchmarkdotnet.png)

3. Now you have successfully setup your project to run benchmarks, but we don't have any code to benchmark yet. The following code example is copied from the `BenchmarkDotnet` documentation[^8], which benchmarks the cryptographic hash functions [MD5](https://en.wikipedia.org/wiki/MD5) and [SHA256](https://en.wikipedia.org/wiki/SHA-2), and adapted it to our situation.

  The `Md5VsSha256.cs` contains the benchmark setup for the SHA256 and MD5 benchmarks.

  ```csharp
  namespace Workshop.PerformanceMeasuring.BenchmarkDotnetDemo
  {
      using System;
      using System.Security.Cryptography;
      using BenchmarkDotNet.Attributes;
      using BenchmarkDotNet.Running;

      public class Md5VsSha256
      {
          private const int N = 10000;
          private readonly byte[] data;
  
          private readonly SHA256 sha256 = SHA256.Create();
          private readonly MD5 md5 = MD5.Create();
  
          public Md5VsSha256()
          {
              data = new byte[N];
              new Random(42).NextBytes(data);
          }
  
          [Benchmark]
          public byte[] Sha256() => sha256.ComputeHash(data);
  
          [Benchmark]
          public byte[] Md5() => md5.ComputeHash(data);
      }
  }
  ```

  The `Program.cs` contains the `BenchmarkRunner` which runs our benchmarks by calling the `Run<Md5VsSha256>` method.

  ```csharp
  namespace Workshop.PerformanceMeasuring.BenchmarkDotnetDemo
  {
      using BenchmarkDotNet.Running;

      public static class Program
      {
          public static void Main(string[] args)
          {
              var summary = BenchmarkRunner.Run<Md5VsSha256>();
          }
      }
  }
  ```

4. To run the benchmarks we must switch the build process to the `Release` mode. In Rider you can achieve this by right-clicking on the solution, select `Properties...` and change `Configuration and Platform` from `Debug | AnyCPU` to `Release | AnyCPU`[^9]. For Visual Studio you can read the guide from Microsoft[^10]. Otherwise you can type the following command within the folder, where your console application is located:

  ```bash
  > dotnet run -c Release
  ```

  This optimizes the output of the binary build and provides BenchmarkDotNet a solid foundation to benchmark.

5. After all benchmarks has successfully been run, BenchmarkDotNet prints the results in the console. An example would look like following:

  ```bash
  BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3) (Hyper-V)
  Intel Xeon Gold 6144 CPU 3.50GHz, 1 CPU, 8 logical and 4 physical cores
  .NET SDK 8.0.201
    [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX-512F+CD+BW+DQ+VL
    DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX-512F+CD+BW+DQ+VL
  
  
  | Method | Mean     | Error    | StdDev   |
  |------- |---------:|---------:|---------:|
  | Sha256 | 42.71 us | 0.766 us | 1.238 us |
  | Md5    | 17.14 us | 0.107 us | 0.089 us |
  ```

## Boring, I want cool stuff!



[^1]: [Wikipedia article of performance measuring (accessed on 03/11/2024)](https://en.wikipedia.org/wiki/Performance_measurement)
[^2]: [Wikipedia article pf (key) performance indicators (accessed on 03/11/2024)](https://en.wikipedia.org/wiki/Performance_indicator)
[^3]: [Visual Studio performance profiler (accessed on 03/11/2024)](https://learn.microsoft.com/en-us/visualstudio/profiling/profiling-feature-tour?view=vs-2022)
[^4]: [Rider performance profiler (accessed on 03/11/2024)](https://www.jetbrains.com/help/rider/Performance_Profiling.html)
[^5]: [BenchmarkDotnets .NET Foundation homepage (accessed on 03/11/2024)](https://dotnetfoundation.org/projects/project-detail/benchmarkdotnet)
[^6]: [BenchmarkDotnets sponsorships (accessed on 03/11/2024)](https://github.com/dotnet/BenchmarkDotNet?tab=readme-ov-file#sponsors)
[^7]: [BenchmarkDotnet documentation (accessed on 03/11/2024)](https://benchmarkdotnet.org/articles/overview.html)
[^8]: [Benchmark example from BenchmarkDotnet documentation (accessed on 03/11/2014)](https://benchmarkdotnet.org/articles/guides/getting-started.html#step-3-design-a-benchmark)
[^9]: [Switch to Release mode in Rider (accessed on 03/11/2024)](https://www.jetbrains.com/help/rider/Build_Configurations.html)
[^10]: [Switch to Release mode in Visual Studio (accessed on 03/11/2024)](https://learn.microsoft.com/en-us/visualstudio/debugger/how-to-set-debug-and-release-configurations?view=vs-2022)
