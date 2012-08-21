
using org.nflac.structure.util;
using System.IO;
using System;

namespace org.nflac.audioformat
{
    public abstract class WaveHeader
    {
        private const uint WAV_HDR_RIFF = 0x52494646; // "RIFF"
        private const uint WAV_HDR_WAVE = 0x57415645; // "WAVE"
        private const uint WAV_HDR_FMT = 0x666D7420; // "fmt "
        private const uint WAV_HDR_DATA = 0x64617461; // "data"

        private const uint WAV_FORMATEX_SIZE = 16;

        private const ushort WAV_FORMAT_PCM = 1;

        internal const ushort WAV_HEADER_SIZE = 44;

        public abstract ulong TotalSamples { get; }
        public abstract ushort BitsPerSample { get; }
        public abstract ushort NumberOfChannels { get; }
        public abstract uint SampleRate { get; }

        private uint streamSize;

        public uint StreamSize
        {
            get { return streamSize; }
            set { streamSize = value; }
        }

        public void WriteRIFFHeader(Stream outputStream)
        {
            uint dataSize = (uint)Math.Ceiling(((double)TotalSamples * ((double)BitsPerSample / 8d)*(double)(NumberOfChannels)));

            streamSize = dataSize + WAV_HEADER_SIZE;

            uint fileSize = dataSize + 36;

            BitWriter wr = new BitWriter(outputStream);

            wr.WriteBits(WAV_HDR_RIFF, 32);
            wr.WriteLE(fileSize);
            wr.WriteBits(WAV_HDR_WAVE, 32);
            wr.WriteBits(WAV_HDR_FMT, 32);
            wr.WriteLE(WAV_FORMATEX_SIZE);
            wr.WriteLE(WAV_FORMAT_PCM);
            wr.WriteLE(NumberOfChannels);
            wr.WriteLE(SampleRate);
            wr.WriteLE((uint)(SampleRate * BitsPerSample * NumberOfChannels / 8)); //avg bytes
            wr.WriteLE((ushort)(BitsPerSample * NumberOfChannels / 8));
            wr.WriteLE(BitsPerSample);
            wr.WriteBits(WAV_HDR_DATA, 32);
            wr.WriteLE(dataSize);


        }
    }
}

