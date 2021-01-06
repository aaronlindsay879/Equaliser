using Backend.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Backend.FileHandling
{
    public static class Wav
    {
        const int SAMPLERATE_LOCATION = 23;
        const int INCREMENT_LOCATION = 33;
        const int SIZE_LOCATION = 39;
        const int AUDIO_LOCATION = 43;

        /// <summary>
        /// Reads and parses bytes from a byte array, ensuring it's parsed correctly
        /// </summary>
        /// <param name="data">Byte array to read from</param>
        /// <param name="start">Index of first byte</param>
        /// <param name="count">Number of bytes to read</param>
        /// <param name="littleEndian">Whether the data is little endian</param>
        /// <param name="twosComplement">Whether to parse it as a two's complement number</param>
        /// <returns>A parsed long</returns>
        private static long ReadBytes(this byte[] data, int start, int count, bool littleEndian = true, bool twosComplement = false)
        {
            int output = 0;

            //for every byte to read
            for (int i = 0; i < count; i++)
            {
                //shift the current stored output along by 8 bits to "make room" for next byte
                output <<= 8;

                //if little endian, then have to read bytes backwards
                //otherwise just read next byte and add to output
                if (littleEndian)
                    output += data[start + count - i];
                else
                    output += data[start + i];
            }

            //if number is stored with two's complement
            if (twosComplement)
            {
                //find the most significant bit by shifting all the others out of the way
                int msb = output >> (count * 8 - 1);

                //if msb is 1, flip the output's msb to convert to a two's complement number
                output -= msb * (int)Math.Pow(2, count * 8);
            }

            return output;
        }

        /// <summary>
        /// Reads the raw bytes from a wav file, and performs needed checks
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>Byte array of raw data</returns>
        /// <exception cref="ArgumentException">Thrown if file does not exist</exception>
        private static byte[] ReadRarWav(string filePath)
        {
            if (!File.Exists(filePath))
                throw new ArgumentException($"File does not exist at {filePath}");

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
            int samplingRate = (int)data.ReadBytes(SAMPLERATE_LOCATION, 4); //sampling rate in Hz
            int audioSize = (int)data.ReadBytes(SIZE_LOCATION, 4) / 2; //how many audio samples in the file
            int increment = (int)data.ReadBytes(INCREMENT_LOCATION, 2) / 8; //how many bytes between starts of samples

            double[] output = new double[audioSize];
            //iterate through all of the audio samples
            for (int i = 0; i < audioSize; i++)
            {
                //finds the location of sample in raw data by multiplying by increment and shifting along by where audio starts
                int byteLocation = i * increment + AUDIO_LOCATION;
                //finds the max value of a sample to correctly map numbers to [-1, 1]
                double maxValue = Math.Pow(2, increment * 8 - 1);

                //read the raw bytes and divide by the max value to constrain
                output[i] = data.ReadBytes(byteLocation, increment, twosComplement: true) / maxValue;
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
        public static Song ReadWav(string filePath) => ParseWav(ReadRarWav(filePath));
    }
}
