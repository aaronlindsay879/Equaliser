using Backend.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Backend.FileHandling.Tests
{
    [TestClass()]
    public class WavTests
    {
        [TestMethod()]
        public void ReadTest()
        {
            Song song = Wav.Read(@"Files/1.wav");

            Assert.AreEqual(1, song.Data.NumChannels);
            Assert.AreEqual(100, song.Data.SampleRate);
            Assert.AreEqual(16, song.Data.BitsPerSample);
            Assert.AreEqual(100, song.Data.NumSamples);
        }

        [TestMethod()]
        [DataRow(@"Files/1.wav")]
        [DataRow(@"Files/440.wav")]
        [DataRow(@"Files/1 2 3.wav")]
        public void SaveIntegrationTest(string filePath)
        {
            Song origSong = Wav.Read(filePath);
            Wav.Save("temp.wav", origSong);

            Song newSong = Wav.Read("temp.wav");

            Assert.IsTrue(origSong.Equals(newSong));

            File.Delete("temp.wav");
        }
    }
}