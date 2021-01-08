using Backend.Data;
using Backend.FileHandling;
using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendBenchmark
{
    [MemoryDiagnoser]
    public class Benchmark
    {
        [Benchmark(Baseline = true)]
        public void ReadWav()
        {
            Wav.Read(@"C:/Eq/1.wav");
        }
    }
}
