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
            Song oneHertz = FileHandling.Wav.Read(@"../../../Files/1.wav");
            DecomposedSongSegment dss = oneHertz.DFT();

            int i = 0;
            foreach (var x in dss.Audio)
            {
                Console.WriteLine($"{i++}: {x}");
            }
        }
    }
}