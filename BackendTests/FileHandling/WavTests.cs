using Microsoft.VisualStudio.TestTools.UnitTesting;
using Backend.FileHandling;
using System;
using System.Collections.Generic;
using System.Text;
using Backend.Data;
using System.IO;

namespace Backend.FileHandling.Tests
{
    [TestClass()]
    public class WavTests
    {
        [TestMethod()]
        public void ReadTest()
        {
            Song song = Wav.Read(@"../../../Files/1.wav");

            Assert.AreEqual(1, song.Data.NumChannels);
            Assert.AreEqual(100, song.Data.SampleRate);
            Assert.AreEqual(16, song.Data.BitsPerSample);
            Assert.AreEqual(100, song.Data.NumSamples);
        }

        [TestMethod()]
        public void SaveIntegrationTest()
        {
            Song origSong = Wav.Read(@"../../../Files/1.wav");
            Wav.Save("temp.wav", origSong);

            Song newSong = Wav.Read("temp.wav");

            Assert.IsTrue(origSong.Equals(newSong));

            File.Delete("temp.wav");
        }
    }
}