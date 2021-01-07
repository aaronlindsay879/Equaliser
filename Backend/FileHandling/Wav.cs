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
            GenerateHeaders(audioData);

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
        public static Song Read(string filePath) => ParseWav(ReadRaw(filePath));

        /// <summary>
        /// A method to generate the headers for a given set of audio data
        /// </summary>
        /// <param name="data">Audio data to construct headers for</param>
        /// <returns>A byte array of headers</returns>
        public static byte[] GenerateHeaders(AudioData data)
        {
            byte[] headers = new byte[AUDIO_LOCATION + 1];

            /* RIFF header
             * 0-3 "RIFF"
             * 4-7 size of everything after here (36 + Subchunk2Size)
             * 8-11 "WAVE"
             */
            headers.SpliceString("RIFF", 0);
            headers.SpliceNum(36 + data.Subchunk2Size, 4, 4);
            headers.SpliceString("WAVE", 8);

            /*FMT sub-chunk
             * 12-15 "fmt "
             * 16-19 should always be the number 16
             * 20-21 should always be the number 1
             * 22-23 number of channels
             * 24-27 sample rate
             * 28-31 byte rate (sample rate * num channels * bytes per sample)
             * 32-33 block align (num channels * bytes per sample)
             * 34-35 bits per sample
             */
            headers.SpliceString("fmt ", 12);
            headers.SpliceNum(16, 16, 4);
            headers.SpliceNum(1, 20, 2);
            headers.SpliceNum(data.NumChannels, 22, 2);
            headers.SpliceNum(data.SampleRate, 24, 4);
            headers.SpliceNum(data.ByteRate, 28, 4);
            headers.SpliceNum(data.NumChannels * data.BytesPerSample, 32, 2);
            headers.SpliceNum(data.BitsPerSample, 34, 2);

            /* DATA sub-chunk
             * 36-39 "data"
             * 40-43 Subchunk2Size (num samples * num channels * bytes per sample)
             * 44+   audio data
             */
            headers.SpliceString("data", 36);
            headers.SpliceNum(data.Subchunk2Size, 40, 4);

            //debug hexdump code
            string output = "";
            foreach (var (val, i) in headers.Select((x, i) => (x.ToString("X2"), i))) output += i % 16 == 0 ? $"\n{val}" : (i % 8 == 0 ? @$"    {val}" : @$"  {val}");

            return headers;
        }
    }
}