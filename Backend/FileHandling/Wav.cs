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
        private const int CHANNELS_LOCATION = 21;
        private const int CHANNELS_BITS = 2;

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
        /// Parses the headers of a wav file to extract useful information
        /// </summary>
        /// <param name="data">Header data</param>
        /// <returns>Important audio data</returns>
        private static AudioData ParseHeaders(byte[] data)
        {
            //fetch simple data from headers
            var audioData = new AudioData
            {
                NumChannels = (short)ReadBytes(data, CHANNELS_LOCATION, CHANNELS_BITS),
                SampleRate = (int)ReadBytes(data, SAMPLERATE_LOCATION, SAMPLERATE_BITS),
                BitsPerSample = (short)ReadBytes(data, INCREMENT_LOCATION, INCREMENT_BITS)
            };

            //calculate number of samples from data in headers and previous data found
            audioData.NumSamples = (int)ReadBytes(data, SIZE_LOCATION, SIZE_BITS);
            audioData.NumSamples /= audioData.NumChannels * audioData.BitsPerSample / 8;

            return audioData;
        }

        /// <summary>
        /// Parses the raw bytes of a WAV file to extract header and audio data
        /// </summary>
        /// <param name="data">Raw bytes of a WAV file</param>
        /// <returns>A song containing time-amplitude data</returns>
        private static Song ParseWav(byte[] data)
        {
            AudioData audioData = ParseHeaders(data.Take(AUDIO_LOCATION + 1).ToArray());

            double[] output = new double[audioData.NumSamples];
            //iterate through all of the audio samples
            for (int i = 0; i < audioData.NumSamples; i++)
            {
                //finds the location of sample in raw data by multiplying by bytes per sample and shifting along by where audio starts
                int byteLocation = i * audioData.BytesPerSample + AUDIO_LOCATION;
                //finds the max value of a sample to correctly map numbers to [-1, 1]
                double maxValue = Math.Pow(2, audioData.BitsPerSample - 1);

                //read the raw bytes and divide by the max value to constrain
                output[i] = ReadBytes(data, byteLocation, audioData.BytesPerSample, twosComplement: true) / maxValue;
            }

            return new Song
            {
                Data = audioData,
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
