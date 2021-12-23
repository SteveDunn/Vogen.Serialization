using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = ManualConfig
                .Create(DefaultConfig.Instance);
                // .WithOptions(ConfigOptions.DisableOptimizationsValidator);
            
            _ = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(
                args,
                config);
        }
    }

}