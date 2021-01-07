using Backend.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Backend.FileHandling
{
    public class Wav : Audio, IAudio
    {
        private const int SAMPLERATE_LOCATION = 23;
        private const int SAMPLERATE_BITS = 4;

        private const int INCREMENT_LOCATION = 33;
        private const int INCREMENT_BITS = 2;

        private const int SIZE_LOCATION = 39;
        private const int SIZE_BITS = 4;

        private const int AUDIO_LOCATION = 43;

        /// <summary>
        /// Reads the raw bytes from a wav file, and performs needed checks
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>Byte array of raw data</returns>
        /// <exception cref="ArgumentException">Thrown if file does not exist</exception>
        private static byte[] ReadRarWav(string filePath)
        {
            if (!File.Exists(filePath))
                throw new ArgumentException($"File does not exist or is inaccessible at {filePath}");

            return File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// Parses the raw bytes of a WAV file to extract header and audio data
        /// </summary>
        /// <param name="data">Raw bytes of a WAV file</param>
        /// <returns>A song containing time-amplitude data</returns>
        private static Song ParseWav(byte[] data)
        {
            //want to keep reference of important locations
            /*Console.WriteLine($"format: {data.ReadBytes(19, 2)}");
            Console.WriteLine($"channels: {data.ReadBytes(21, 2)}");
            Console.WriteLine($"sample rate: {data.ReadBytes(23, 4)}");
            Console.WriteLine($"byte rate: {data.ReadBytes(27, 4)}");
            Console.WriteLine($"block align: {data.ReadBytes(31, 2)}");
            Console.WriteLine($"bits per sample: {data.ReadBytes(33, 2)}");
            Console.WriteLine($"audio size: {data.ReadBytes(39, 4)}");*/

            //fetches data from headers needed for parsing
            int samplingRate = (int)ReadBytes(data, SAMPLERATE_LOCATION, SAMPLERATE_BITS); //sampling rate in Hz
            int audioSize = (int)ReadBytes(data, SIZE_LOCATION, SIZE_BITS) / 2; //how many audio samples in the file
            int increment = (int)ReadBytes(data, INCREMENT_LOCATION, INCREMENT_BITS) / 8; //how many bytes between starts of samples

            double[] output = new double[audioSize];
            //iterate through all of the audio samples
            for (int i = 0; i < audioSize; i++)
            {
                //finds the location of sample in raw data by multiplying by increment and shifting along by where audio starts
                int byteLocation = i * increment + AUDIO_LOCATION;
                //finds the max value of a sample to correctly map numbers to [-1, 1]
                double maxValue = Math.Pow(2, increment * 8 - 1);

                //read the raw bytes and divide by the max value to constrain
                output[i] = ReadBytes(data, byteLocation, increment, twosComplement: true) / maxValue;
            }

            return new Song
            {
                SamplingRate = samplingRate,
                Sound = output
            };
        }

        /// <summary>
        /// Reads and parses a wav file
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <returns>A song containing time-amplitude data</returns>
        public static Song Read(string filePath) => ParseWav(ReadRarWav(filePath));
    }
}
