using Microsoft.VisualStudio.TestTools.UnitTesting;
using Backend.Algorithms;
using System;
using System.Collections.Generic;
using System.Text;
using Backend.Data;
using System.Linq;

namespace Backend.Algorithms.Tests
{
    [TestClass()]
    public class FourierTests
    {
        [TestMethod()]
        public void DFTTest()
        {
            /*            Song song = new Song()
                        {
                            SamplingRate = 10,
                            Sound = Enumerable.Range(0, 100).Select(x => Math.Sin(x * Math.PI / 25)).ToArray()
                        };

                        foreach (var (x, i) in song.DFT().Audio.Select((x, i) => (x, i)))
                        {
                            Console.WriteLine($"Freq: {i}, Amp: {x}");
                        }*/


            byte[] data = FileHandling.Wav.ReadWav("../../../Files/440.wav");
            FileHandling.Wav.ParseWav(data);
        }
    }
}