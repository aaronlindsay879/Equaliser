using System;
using System.Text;

namespace Backend.FileHandling
{
    public static class Extensions
    {
        /// <summary>
        /// Splices a string into a byte array, by initially converting string to an ASCII byte array
        /// </summary>
        /// <param name="arr">Byte array to splice into</param>
        /// <param name="str">String to splice into array</param>
        /// <param name="start">Where to place first string character</param>
        public static void SpliceString(this byte[] arr, string str, int start)
        {
            //converts string to an ascii byte array
            byte[] byteString = Encoding.ASCII.GetBytes(str);

            //copies the string into the byte array at given positions
            Array.Copy(byteString, 0, arr, start, byteString.Length);
        }

        /// <summary>
        /// Splices a number into a byte array, by converting number to a byte array with given arguments
        /// </summary>
        /// <param name="arr">Byte array to splice into</param>
        /// <param name="num">Number to splice into array</param>
        /// <param name="start">Where to place first byte of number</param>
        /// <param name="count">How many bytes to write the number into</param>
        /// <param name="littleEndian">Whether to use little endian</param>
        public static void SpliceNum(this byte[] arr, long num, int start, int count, bool littleEndian = true)
        {
            //convert the number to a byte array using given arguments
            byte[] byteNum = Audio.WriteBytes(num, count, littleEndian);

            //copies the number into byte array at given positions
            Array.Copy(byteNum, 0, arr, start, count);
        }
    }
}
