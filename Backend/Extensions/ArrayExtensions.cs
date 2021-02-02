using Backend.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Splices a string into a byte array, by initially converting string to an ASCII byte array
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="arr">Byte array to splice into</param>
        /// <param name="str">String to splice into array</param>'
        /// <param name="enumValue">Value of the enum that contains info on where to write</param>
        public static void SpliceString<T>(this byte[] arr, string str, T enumValue) where T : IConvertible
        {
            var info = enumValue.BitInfo();

            //converts string to an ascii byte array
            byte[] byteString = Encoding.ASCII.GetBytes(str);

            //copies the string into the byte array at given positions
            Array.Copy(byteString, 0, arr, info.Location, byteString.Length);
        }

        /// <summary>
        /// Splices a number into a byte array, by converting number to a byte array with given arguments
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="arr">Byte array to splice into</param>
        /// <param name="num">Number to splice into array</param>
        /// <param name="enumValue">Value of the enum that contains info on where to write</param>
        /// <param name="littleEndian">Whether to use little endian</param>
        public static void SpliceNum<T>(this byte[] arr, long num, T enumValue, bool littleEndian = true) where T : IConvertible
        {
            var info = enumValue.BitInfo();
            arr.SpliceNum(num, info.Location, info.Offset, littleEndian);
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
