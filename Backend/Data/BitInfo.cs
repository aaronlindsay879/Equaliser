using System;

namespace Backend.Data
{
    /// <summary>
    /// Struct containing information on the location and size of data within a byte array
    /// </summary>
    public struct BitInfo
    {
        /// <summary>
        /// Location of the first bit of information
        /// </summary>
        public int Location;

        /// <summary>
        /// How many bytes the information consists of
        /// </summary>
        public int Offset;

        /// <summary>
        /// Constructs a BitInfo from a tuple assignment
        /// </summary>
        /// <param name="tuple">Tuple wherre item1 = location, item2 = bit length</param>
        public static implicit operator BitInfo((int, int) tuple) => new BitInfo
        {
            Location = tuple.Item1,
            Offset = tuple.Item2
        };
    }

    public class EnumInfo : Attribute
    {
        public readonly BitInfo Info;

        public EnumInfo(int location, int bytes) => Info = new BitInfo() { Location = location, Offset = bytes };
    }
}
