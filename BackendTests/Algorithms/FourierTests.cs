using Backend.Data;
using Backend.FileHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Backend.Algorithms.Tests
{
    [TestClass]
    public class FourierTests
    {
        [TestMethod]
        [DataRow("Files/1.wav", new double[] {0, 0.8})]
        [DataRow("Files/1 4.wav", new double[] { 0, 0.5, 0, 0, 0.5 })]
        [DataRow("Files/1 2 3.wav", new double[] { 0, 0.4, 0.4, 0.4 })]
        public void DFTTest(string path, double[] output)
        {
            Song song = Wav.Read(path);
            DecomposedSongSegment dss = song.DFT();

            foreach (var (amplitude, index) in dss.Audio.Select((x, i) => (x, i)))
            {
                if (index > output.Length - 1)
                    Assert.AreEqual(0, amplitude, 5e-4);
                else
                    Assert.AreEqual(output[index], amplitude, 5e-4);
            }
            
        }
    }
}