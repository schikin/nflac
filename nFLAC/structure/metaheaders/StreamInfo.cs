using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders
{
    public class StreamInfo : Metadata
    {
        private ushort minimumBlockSize;
        private ushort maximumBlockSize;

        private uint minimumFrameSize;       
        private uint maximumFrameSize;

        private uint sampleRate;
        private byte numberOfChannels;
      
        private byte bitsPerSample;
        private ulong totalSamples;

        private byte[] md5 = new byte[16];

        public ushort MinimumBlockSize
        {
            get { return minimumBlockSize; }
        }

        public ushort MaximumBlockSize
        {
            get { return maximumBlockSize; }
        }

        public uint MinimumFrameSize
        {
            get { return minimumFrameSize; }
        }

        public uint MaximumFrameSize
        {
            get { return maximumFrameSize; }
        }

        public uint SampleRate
        {
            get { return sampleRate; }
        }

        public byte NumberOfChannels
        {
            get { return numberOfChannels; }
        }

        public byte BitsPerSample
        {
            get { return bitsPerSample; }
        }

        public ulong TotalSamples
        {
            get { return totalSamples; }
        }

        public byte[] MD5
        {
            get { return md5; }
        }

        protected override void Parse(byte[] payload)
        {

            minimumBlockSize = payload[0];
            minimumBlockSize = (ushort) (minimumBlockSize << 8);
            minimumBlockSize |= payload[1];

            maximumBlockSize = payload[2];
            maximumBlockSize = (ushort)(maximumBlockSize << 8);
            maximumBlockSize |= payload[3];

            minimumFrameSize = payload[4];
            minimumFrameSize = (uint)(minimumFrameSize << 8);
            minimumFrameSize |= payload[5];
            minimumFrameSize = (uint)(minimumFrameSize << 8);
            minimumFrameSize |= payload[6];

            maximumFrameSize = payload[7];
            maximumFrameSize = (uint)(maximumFrameSize << 8);
            maximumFrameSize |= payload[8];
            maximumFrameSize = (uint)(maximumFrameSize << 8);
            maximumFrameSize |= payload[9];

            sampleRate = payload[10];
            sampleRate = (uint)(sampleRate << 8);
            sampleRate |= payload[11];
            sampleRate = (uint)(sampleRate << 4);
            sampleRate |= (uint) (payload[12] >> 4);

            numberOfChannels = (byte)(((payload[12] & 0xE) >> 1) + 1);

            bitsPerSample = (byte)((payload[12] & 0x1)<<4);
            bitsPerSample |= (byte)(payload[13] >> 4);
            bitsPerSample++;
            
            totalSamples = (ulong)(payload[13] & 0xF);
            totalSamples = totalSamples << 8;
            totalSamples |= (ulong)(payload[14]);
            totalSamples = totalSamples << 8;
            totalSamples |= (ulong)(payload[15]);
            totalSamples = totalSamples << 8;
            totalSamples |= (ulong)(payload[16]);
            totalSamples = totalSamples << 8;
            totalSamples |= (ulong)(payload[17]);

            for (int i = 0; i < 16; i++)
            {
                md5[i] = payload[i + 18];
            }
            
        }

        public override string ToString()
        {
            string ret;

            ret = base.ToString();

            ret += "\n Minimum block size: " + minimumBlockSize + " samples";
            ret += "\n Maximum block size: " + maximumBlockSize + " samples";

            ret += "\n Minimum frame size: " + minimumFrameSize + " bytes";
            ret += "\n Maximum frame size: " + maximumFrameSize + " bytes";

            ret += "\n Sample rate: " + sampleRate + " Hz";
            ret += "\n Number of channels: " + numberOfChannels;

            ret += "\n Bits per sample: " + bitsPerSample;
            ret += "\n Total samples: " + totalSamples;

            ret += "\n MD5: " + BitConverter.ToString(md5).Replace("-", "").ToLower();



            return ret;
        }
    }
}
