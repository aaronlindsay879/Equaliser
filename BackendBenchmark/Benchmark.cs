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
        //NAME                      TIME  MEM
        //array copy                722us 10.7kb
        //byte concat before return 727us 10.8kb
        //old                       835us 10.8kb
        [Benchmark(Baseline = true)]
        public void ReadSaveWav()
        {
            Wav.Save("temp.wav", Wav.Read(@"C:/Eq/440.wav"));
        }
    }
}
