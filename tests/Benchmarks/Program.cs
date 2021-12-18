using BenchmarkDotNet.Running;

namespace Benchmarks
{
    public class Program
    {
        public static void Main(string[] args) => 
            _ = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }

}