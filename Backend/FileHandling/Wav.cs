using Backend.Data;
using System;
using System.Linq;

namespace Backend.FileHandling
{
    public class Wav : Audio, IAudio
    {
        private static readonly BitInfo CHANNEL = (22, 2);
        private static readonly BitInfo SAMPLERATE = (24, 4);
        private static readonly BitInfo INCREMENT = (34, 2);
        private static readonly BitInfo SIZE = (40, 4);

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
                NumChannels = (short)ReadBytes(data, CHANNEL),
                SampleRate = (int)ReadBytes(data, SAMPLERATE),
                BitsPerSample = (short)ReadBytes(data, INCREMENT)
            };

            //calculate number of samples from data in headers and previous data found
            audioData.NumSamples = (int)ReadBytes(data, SIZE);
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

            //finds the max value of a sample to correctly map numbers to [-1, 1]
            double maxValue = Math.Pow(2, audioData.BitsPerSample - 1);

            double[] output = new double[audioData.NumSamples];
            //iterate through all of the audio samples
            for (int i = 0; i < audioData.NumSamples; i++)
            {
                //finds the location of sample in raw data by multiplying by bytes per sample and shifting along by where audio starts
                int byteLocation = i * audioData.BytesPerSample + AUDIO_LOCATION;

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
            byte[] headers = new byte[AUDIO_LOCATION + 1];

            /* 
             * RIFF header
             * 0-3 "RIFF"
             * 4-7 size of everything after here (36 + Subchunk2Size)
             * 8-11 "WAVE"
             */
            headers.SpliceString("RIFF", 0);
            headers.SpliceNum(36 + data.Subchunk2Size, 4, 4);
            headers.SpliceString("WAVE", 8);

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
            headers.SpliceString("fmt ", 12);
            headers.SpliceNum(16, 16, 4);
            headers.SpliceNum(1, 20, 2);
            headers.SpliceNum(data.NumChannels, 22, 2);
            headers.SpliceNum(data.SampleRate, 24, 4);
            headers.SpliceNum(data.ByteRate, 28, 4);
            headers.SpliceNum(data.NumChannels * data.BytesPerSample, 32, 2);
            headers.SpliceNum(data.BitsPerSample, 34, 2);

            /*
             * DATA sub-chunk
             * 36-39 "data"
             * 40-43 Subchunk2Size (num samples * num channels * bytes per sample)
             * 44+   audio data
             */
            headers.SpliceString("data", 36);
            headers.SpliceNum(data.Subchunk2Size, 40, 4);

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

            //for each (sample, index) in the sound - standard index (0, 1, 2, etc.)
            foreach (var (sample, index) in song.Sound.Select((x, i) => (x, i)))
            {
                //multiply the double by 2^(bits per sample - 1) to move back to storage types
                long num = (long)(sample * (1 << (song.Data.BitsPerSample - 1)));

                //if num is less than 0, add value of msb to make it positive again
                //essentially reverse two's complement
                if (num < 0)
                    num += 1 << song.Data.BitsPerSample;

                //splice the calculate number into the byte array
                output.SpliceNum(num, index * song.Data.BytesPerSample, song.Data.BytesPerSample);
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
            byte[] data = new byte[song.Data.NumSamples * song.Data.BytesPerSample + AUDIO_LOCATION + 1];

            //write headers to first 44 bytes
            Array.Copy(GenerateHeaders(song.Data), 0, data, 0, AUDIO_LOCATION + 1);
            //fill remaining data with audio bytes
            Array.Copy(GenerateAudioBytes(song), 0, data, AUDIO_LOCATION + 1, song.Data.NumSamples * song.Data.BytesPerSample);

            //write data
            return WriteRaw(filePath, data);
        }
    }
}