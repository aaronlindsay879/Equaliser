using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.FileHandling
{
    public class Audio
    {
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
    }
}
