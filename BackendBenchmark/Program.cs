using BenchmarkDotNet.Running;
using System;

namespace BackendBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmark>();
        }
    }
}
