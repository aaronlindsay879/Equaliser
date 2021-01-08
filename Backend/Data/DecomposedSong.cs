using System.Collections.Generic;

namespace Backend.Data
{
    /// <summary>
    /// A class represent a song decomposed into frequency-amplitude pairs at one specific time.
    /// The value of Audio at n is equal to the amplitude of the audio at the frequency n.
    /// </summary>
    public class DecomposedSongSegment
    {
        public double[] Audio;
        public int SamplingRate;

        /// <summary>
        /// Constructs a decomposed song segment with a given length for audio array
        /// </summary>
        /// <param name="length">Length of audio array</param>
        public DecomposedSongSegment(int length)
        {
            Audio = new double[length];
        }
    }

    /// <summary>
    /// A collection of decomposed audio segments.
    /// </summary>
    public class DecomposedSong
    {
        public Dictionary<double, DecomposedSongSegment> Audio;
        public int SamplingRate;
    }
}
