namespace Workshop.PerformanceMeasuring.BenchmarkDotNetAdvancedDemo
{
    using System.Security.Cryptography;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Jobs;

    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net70)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [CsvMeasurementsExporter]
    public class Md5VsSha256
    {
        private byte[] data = Array.Empty<byte>();

        private readonly SHA256 sha256 = SHA256.Create();
        private readonly MD5 md5 = MD5.Create();
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            this.data = new byte[N];
            new Random(42).NextBytes(data);
        }
        
        [Params(100, 1_000, 10_000)]
        public int N { get; set; }

        [Benchmark]
        public byte[] Sha256() => sha256.ComputeHash(data);

        [Benchmark]
        public byte[] Md5() => md5.ComputeHash(data);
    }
}