using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Data
{
    public class AudioData
    {
        public short NumChannels;
        public int SampleRate;
        public short BitsPerSample;
        public int NumSamples;

        public short BytesPerSample => (short)(BitsPerSample / 8);
    }
}
