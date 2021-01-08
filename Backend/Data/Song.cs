using System;
using System.Linq;

namespace Backend.Data
{
    public class Song : IEquatable<Song>
    {
        /// <summary>
        /// The actual sound data, values lie in range [-1, 1]
        /// </summary>
        public double[] Sound;

        /// <summary>
        /// The metadata for the song
        /// </summary>
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
