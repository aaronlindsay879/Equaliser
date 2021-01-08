using System;
using System.Linq;

namespace Backend.Data
{
    public class Song : IEquatable<Song>
    {
        public double[] Sound;
        public AudioData Data;

        //kept in temp
        public int SamplingRate;

        public bool Equals(Song other)
        {
            return Enumerable.SequenceEqual(Sound, other.Sound)
                && Data == other.Data;
        }
    }
}
