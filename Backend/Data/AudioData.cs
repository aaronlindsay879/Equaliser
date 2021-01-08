namespace Backend.Data
{
    /// <summary>
    /// A record containing important metadata for audio files
    /// </summary>
    public record AudioData
    {
        /// <summary>
        /// Numbers of channels in the audio file
        /// </summary>
        public short NumChannels;
        
        /// <summary>
        /// How many samples correspond to one second of audio
        /// </summary>
        public int SampleRate;

        /// <summary>
        /// How many bits there are per audio sample (typically 16)
        /// </summary>
        public short BitsPerSample;

        /// <summary>
        /// How many samples there are after the header
        /// </summary>
        public int NumSamples;


        /// <summary>
        /// The number of bytes per audio sample (typically 2)
        /// </summary>
        public short BytesPerSample => (short)(BitsPerSample / 8);

        /// <summary>
        /// The number of bytes after the header
        /// </summary>
        public int Subchunk2Size => NumSamples * NumChannels * BytesPerSample;

        /// <summary>
        /// How many bytes correspond to one second of audio
        /// </summary>
        public int ByteRate => SampleRate * NumChannels * BytesPerSample;
    }
}
