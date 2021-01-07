using Backend.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Algorithms
{
    public static class Fourier
    {
        public static DecomposedSongSegment DFT(this Song song)
        {
            //only frequences up to half of the sampling rate can be represented
            int maxFreq = song.Sound.Length / 2;

            DecomposedSongSegment output = new DecomposedSongSegment(maxFreq) { SamplingRate = song.SamplingRate };

            //for every frequency we can calculate
            for (int freq = 0; freq < maxFreq; freq += song.SamplingRate / song.Sound.Length)
            {
                Complex tempOutput = new Complex();
                
                //calculate the raw output at a given frequency using the standard formula for discrete fourier transform
                for (int sample = 0; sample < song.Sound.Length; sample++)
                {
                    double angle = -2d * Math.PI * freq * sample / song.Sound.Length;
                    tempOutput += song.Sound[sample] * new Complex(angle);
                }

                //convert raw output to useful value
                int index = freq == 0 ? 0 : freq / (song.SamplingRate / song.Sound.Length);
                output.Audio[index] = Math.Round(2 * tempOutput.Magnitude / song.Sound.Length, 5);
            }

            return output;
        }
    }
}
