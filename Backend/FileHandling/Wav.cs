using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Backend.FileHandling
{
    public static class Wav
    {
        private static long ReadBytes(this byte[] data, int start, int count, bool littleEndian = true)
        {
            int output = 0;

            for (int i = 0; i < count; i++)
            {
                output <<= 8;

                if (littleEndian)
                    output += data[start + count - i];
                else
                    output += data[start + i];
            }

            return output;
        }

        public static byte[] ReadWav(string filePath)
        {
            if (!File.Exists(filePath))
                throw new ArgumentException($"File does not exist at {filePath}");

            return File.ReadAllBytes(filePath);
        }

        public static byte[] ParseWav(byte[] data)
        {
            Console.WriteLine($"format: {data.ReadBytes(19, 2)}");
            Console.WriteLine($"channels: {data.ReadBytes(21, 2)}");
            Console.WriteLine($"sample rate: {data.ReadBytes(23, 4)}");
            Console.WriteLine($"byte rate: {data.ReadBytes(27, 4)}");
            Console.WriteLine($"block align: {data.ReadBytes(31, 2)}");
            Console.WriteLine($"bits per sample: {data.ReadBytes(33, 2)}");

            /*for (int i = 0; i < 44; i++)
                Console.WriteLine($"{i}: {data[i]:X}");*/

            return data;
        }
    }
}
