using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Backend.Data
{
    public class AudioData : IEquatable<AudioData>
    {
        public short NumChannels;
        public int SampleRate;
        public short BitsPerSample;
        public int NumSamples;

        public short BytesPerSample => (short)(BitsPerSample / 8);
        public int Subchunk2Size => NumSamples * NumChannels * BytesPerSample;
        public int ByteRate => SampleRate * NumChannels * BytesPerSample;

        public bool Equals(AudioData other)
        {
            return NumChannels == other.NumChannels
                && SampleRate == other.SampleRate
                && BitsPerSample == other.BitsPerSample
                && NumSamples == other.NumSamples;
        }
    }
}
