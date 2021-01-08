using Backend.Data;
using System;
using System.IO;

namespace Backend.FileHandling
{
    public class Audio
    {
        /// <summary>
        /// Reads the raw bytes from an audio file, and performs needed checks
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>Byte array of raw data</returns>
        /// <exception cref="ArgumentException">Thrown if file does not exist</exception>
        protected static byte[] ReadRaw(string filePath)
        {
            if (!File.Exists(filePath))
                throw new ArgumentException($"File does not exist or is inaccessible at {filePath}");

            return File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// Tries to write an array of bytes to a given file path
        /// </summary>
        /// <param name="filePath">File path to write to</param>
        /// <param name="bytes">Bytes to write</param>
        /// <returns>Bool indicating success</returns>
        protected static bool WriteRaw(string filePath, byte[] bytes)
        {
            try
            {
                File.WriteAllBytes(filePath, bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reads and parses bytes from a byte array, ensuring it's parsed correctly
        /// </summary>
        /// <param name="data">Byte array to read from</param>
        /// <param name="start">Index of first byte</param>
        /// <param name="count">Number of bytes to read</param>
        /// <param name="littleEndian">Whether the data is little endian</param>
        /// <param name="twosComplement">Whether to parse it as a two's complement number</param>
        /// <returns>A parsed long</returns>
        protected static long ReadBytes(byte[] data, int start, int count, bool littleEndian = true, bool twosComplement = false)
        {
            long output = 0;

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
                int msb = (int)(output >> (count * 8 - 1));

                //if msb is 1, flip the output's msb to convert to a two's complement number
                output -= msb * (1 << (count * 8));
            }

            return output;
        }

        /// <summary>
        /// Reads and parses bytes from a byte array, ensuring it's parsed correctly
        /// </summary>
        /// <param name="data">Byte array to read from</param>
        /// <param name="info">Information on where to read</param>
        /// <param name="littleEndian">Whether the data is little endian</param>
        /// <param name="twosComplement">Whether to parse it as a two's complement number</param>
        /// <returns>A parsed long</returns>
        protected static long ReadBytes(byte[] data, BitInfo info, bool littleEndian = true, bool twosComplement = false) =>
            ReadBytes(data, info.Location - 1, info.Bits, littleEndian, twosComplement);

        /// <summary>
        /// Writes given data to an array of bytes
        /// </summary>
        /// <param name="data">Data to write</param>
        /// <param name="count">Number of bytes</param>
        /// <param name="littleEndian">Whether to use little or big endian</param>
        /// <returns>A byte array containing the value <paramref name="data"/></returns>
        public static byte[] WriteBytes(long data, int count, bool littleEndian = true)
        {
            byte[] output = new byte[count];

            for (int i = 0; i < count; i++)
            {
                //calculate how far to bitshift the data. this decides which bits get masked via the bitmask
                //if using big endian, have to mask values in reverse
                int shift;
                if (littleEndian)
                    shift = i * 8;
                else
                    shift = (count - 1) * 8 - i * 8;

                //shift the value to the correct position and mask by 0xff (0b11111111) to isolate last 8 bits
                //then put the byte in the array
                output[i] = (byte)(data >> shift & 0xff);
            }

           return output;
        }
    }
}
