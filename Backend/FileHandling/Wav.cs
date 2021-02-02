using Backend.Data;
using Backend.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Backend.FileHandling
{
    enum WavData
    {
        [EnumInfo(0, 4)] CHUNK_ID,
        [EnumInfo(4, 4)] CHUNK_SIZE,
        [EnumInfo(8, 4)] FORMAT,
        [EnumInfo(12, 4)] SUBCHUNK_1_ID,
        [EnumInfo(16, 4)] SUBCHUNK_1_SIZE,
        [EnumInfo(20, 4)] AUDIO_FORMAT,
        [EnumInfo(22, 2)] NUM_CHANNELS,
        [EnumInfo(24, 4)] SAMPLE_RATE,
        [EnumInfo(28, 4)] BYTE_RATE,
        [EnumInfo(32, 2)] BLOCK_ALIGN,
        [EnumInfo(34, 2)] BITS_PER_SAMPLE,
        [EnumInfo(36, 4)] SUBCHUNK_2_ID,
        [EnumInfo(40, 4)] SUBCHUNK_2_SIZE,
        [EnumInfo(44, -1)] DATA
    }

    public class Wav : Audio, IAudio
    {
        private static readonly int DATA_LOCATION = WavData.DATA.BitInfo().Location;

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
                NumChannels = (short)ReadBytes(data, WavData.NUM_CHANNELS),
                SampleRate = (int)ReadBytes(data, WavData.SAMPLE_RATE),
                BitsPerSample = (short)ReadBytes(data, WavData.BITS_PER_SAMPLE)
            };

            //calculate number of samples from data in headers and previous data found
            audioData.NumSamples = (int)ReadBytes(data, WavData.SUBCHUNK_2_SIZE);
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
            AudioData audioData = ParseHeaders(data.Take(DATA_LOCATION + 1).ToArray());
            GenerateHeaders(audioData);

            //finds the max value of a sample to correctly map numbers to [-1, 1]
            double maxValue = Math.Pow(2, audioData.BitsPerSample - 1);

            double[] output = new double[audioData.NumSamples];
            //iterate through all of the audio samples
            for (int i = 0; i < audioData.NumSamples; i++)
            {
                //finds the location of sample in raw data by multiplying by bytes per sample and shifting along by where audio starts
                int byteLocation = i * audioData.BytesPerSample + DATA_LOCATION;

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
        private static byte[] GenerateHeaders(AudioData data)
        {
            byte[] headers = new byte[DATA_LOCATION + 1];

            /* 
             * RIFF header
             * 0-3 "RIFF"
             * 4-7 size of everything after here (36 + Subchunk2Size)
             * 8-11 "WAVE"
             */
            headers.SpliceString("RIFF", WavData.CHUNK_ID);
            headers.SpliceNum(36 + data.Subchunk2Size, WavData.CHUNK_SIZE);
            headers.SpliceString("WAVE", WavData.FORMAT);

            /* 
             * FMT sub-chunk
             * 12-15 "fmt "
             * 16-19 should always be the number 16
             * 20-21 should always be the number 1
             * 22-23 number of channels
             * 24-27 sample rate
             * 28-31 byte rate (sample rate * num channels * bytes per sample)
             * 32-33 block align (num channels * bytes per sample)
             * 34-35 bits per sample
             */
            headers.SpliceString("fmt ", WavData.SUBCHUNK_1_ID);
            headers.SpliceNum(16, WavData.SUBCHUNK_1_SIZE);
            headers.SpliceNum(1, WavData.AUDIO_FORMAT);
            headers.SpliceNum(data.NumChannels, WavData.NUM_CHANNELS);
            headers.SpliceNum(data.SampleRate, WavData.SAMPLE_RATE);
            headers.SpliceNum(data.ByteRate, WavData.BYTE_RATE);
            headers.SpliceNum(data.NumChannels * data.BytesPerSample, WavData.BLOCK_ALIGN);
            headers.SpliceNum(data.BitsPerSample, WavData.BITS_PER_SAMPLE);

            /*
             * DATA sub-chunk
             * 36-39 "data"
             * 40-43 Subchunk2Size (num samples * num channels * bytes per sample)
             * 44+   audio data
             */
            headers.SpliceString("data", WavData.SUBCHUNK_2_ID);
            headers.SpliceNum(data.Subchunk2Size, WavData.SUBCHUNK_2_SIZE);

            return headers;
        }

        /// <summary>
        /// Generates the bytes for the actual audio of the song
        /// </summary>
        /// <param name="song">Song to generate bytes for</param>
        /// <returns>Byte array of audio</returns>
        private static byte[] GenerateAudioBytes(Song song)
        {
            //generate a byte array with sufficient size for all data (num samples * bytes per sample)
            byte[] output = new byte[song.Data.NumSamples * song.Data.BytesPerSample];

            //for each sample in the sound
            int index = 0;
            foreach (var sample in song.Sound)
            {
                //multiply the double by 2^(bits per sample - 1) to move back to storage types
                long num = (long)(sample * (1 << (song.Data.BitsPerSample - 1)));

                //if num is less than 0, add value of msb to make it positive again
                //essentially reverse two's complement
                if (num < 0)
                    num += 1 << song.Data.BitsPerSample;

                //splice the calculate number into the byte array
                output.SpliceNum(num, index++ * song.Data.BytesPerSample, song.Data.BytesPerSample);
            }

            return output;
        }

        /// <summary>
        /// Saves a given song to a given location
        /// </summary>
        /// <param name="filePath">Location to save song to</param>
        /// <param name="song">Song to save</param>
        public static bool Save(string filePath, Song song)
        {
            //make a byte array of sufficient length
            byte[] data = new byte[song.Data.NumSamples * song.Data.BytesPerSample + DATA_LOCATION + 1];

            //write headers to first 44 bytes
            Array.Copy(GenerateHeaders(song.Data), 0, data, 0, DATA_LOCATION + 1);
            //fill remaining data with audio bytes
            Array.Copy(GenerateAudioBytes(song), 0, data, DATA_LOCATION + 1, song.Data.NumSamples * song.Data.BytesPerSample);

            //write data
            return WriteRaw(filePath, data);
        }
    }
}