using Org.Nflac.Audioformat;
using Org.Nflac.Flac.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Org.Nflac.Wave
{
    public class WaveStreamInfo : StreamInfo
    {
        private static readonly uint WAV_HDR_RIFF = 0x52494646; // "RIFF"
        private static readonly uint WAV_HDR_WAVE = 0x57415645; // "WAVE"
        private static readonly uint WAV_HDR_FMT = 0x666D7420; // "fmt "
        private static readonly uint WAV_HDR_DATA = 0x64617461; // "data"

        private static readonly uint WAV_FORMATEX_SIZE = 16;

        internal static readonly ushort WAV_HEADER_SIZE = 44;

        public WaveCompression AudioFormat { get; set; } //PCM == 1

        public byte[] GetRIFFHeader()
        {
            MemoryStream str = new MemoryStream();

            WriteRIFFHeader(str);

            return str.ToArray();
        }

        public void WriteRIFFHeader(Stream outputStream)
        {
            uint dataSize = (uint)Math.Ceiling(((double)TotalSamples * ((double)BitsPerSample / 8d) * (double)(NumberOfChannels)));

            var streamSize = dataSize + WAV_HEADER_SIZE;

            uint fileSize = dataSize + 36;

            BitWriter wr = new BitWriter(outputStream);

            wr.WriteBits(WAV_HDR_RIFF, 32);
            wr.WriteLE(fileSize);
            wr.WriteBits(WAV_HDR_WAVE, 32);
            wr.WriteBits(WAV_HDR_FMT, 32);
            wr.WriteLE(WAV_FORMATEX_SIZE);
            wr.WriteLE((ushort)AudioFormat);
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
