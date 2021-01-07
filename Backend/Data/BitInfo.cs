using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Data
{
    public struct BitInfo
    {
        public int Location;
        public int Bits;

        public static implicit operator BitInfo((int, int) tuple) => new BitInfo
        {
            Location = tuple.Item1,
            Bits = tuple.Item2
        };
    }
}
