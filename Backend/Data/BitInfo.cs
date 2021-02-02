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

        public BitInfo(int location, int offset) => (Location, Offset) = (location, offset);
    }

    /// <summary>
    /// Attribute class which contains information about the location and offset of data for a given element
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumInfo : Attribute
    {
        public readonly BitInfo Info;

        /// <summary>
        /// Constructs a BitInfo from given parameters
        /// </summary>
        /// <param name="location">Start of byte data</param>
        /// <param name="offset">How many bytes</param>
        public EnumInfo(int location, int offset)
        {
            Info = new(location, offset);
        }
    }
}
