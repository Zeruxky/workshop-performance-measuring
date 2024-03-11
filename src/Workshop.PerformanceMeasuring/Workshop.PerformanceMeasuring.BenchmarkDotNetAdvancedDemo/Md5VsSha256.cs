namespace Workshop.PerformanceMeasuring.BenchmarkDotNetAdvancedDemo
{
    using System.Security.Cryptography;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Jobs;

    [MemoryDiagnoser]
    [ExceptionDiagnoser]
    [CsvMeasurementsExporter]
    [SimpleJob(RuntimeMoniker.Net481)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class Md5VsSha256
    {
        private byte[] data = Array.Empty<byte>();
  
        private readonly SHA256 sha256 = SHA256.Create();
        private readonly MD5 md5 = MD5.Create();
        
        [Params(1, 10, 100, 1_000, 10_000)]
        public int N { get; set; }

        [GlobalSetup]
        public void SetupBenchmark()
        {
            this.data = new byte[N];
            new Random(42).NextBytes(data);
        }

        [GlobalCleanup]
        public void CleanUpBenchmark()
        {
            Array.Clear(this.data);
        }
  
        [Benchmark]
        public byte[] Sha256() => sha256.ComputeHash(data);
  
        [Benchmark]
        public byte[] Md5() => md5.ComputeHash(data);

        [Benchmark]
        public int CodeThatThrowsSometimesExceptions()
        {
            var throwException = Random.Shared.Next(0, 2);
            if (throwException == 1)
            {
                throw new Exception("boom");
            }

            return 1;
        }
    }
}