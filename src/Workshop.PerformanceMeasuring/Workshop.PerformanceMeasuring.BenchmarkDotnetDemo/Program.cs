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