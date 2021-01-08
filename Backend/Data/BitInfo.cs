namespace Backend.Data
{
    /// <summary>
    /// Struct containing information on the location and size of data within a byte array
    /// </summary>
    public struct BitInfo
    {
        public int Location;
        public int Bits;

        /// <summary>
        /// Constructs a BitInfo from a tuple assignment
        /// </summary>
        /// <param name="tuple">Tuple wherre item1 = location, item2 = bit length</param>
        public static implicit operator BitInfo((int, int) tuple) => new BitInfo
        {
            Location = tuple.Item1,
            Bits = tuple.Item2
        };
    }
}
