using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.FileHandling
{
    public static class Extensions
    {
        public static void SpliceString<T>(this T[] arr, string str, int start)
        {
            byte[] byteString = Encoding.ASCII.GetBytes(str);
            Array.Copy(byteString, 0, arr, start, byteString.Length);
        }

        public static void SpliceNum<T>(this T[] arr, long num, int start, int count, bool littleEndian = true, bool twosComplement = false)
        {
            byte[] byteNum = Audio.WriteBytes(num, count, littleEndian, twosComplement);
            Array.Copy(byteNum, 0, arr, start, count);
        }
    }
}
