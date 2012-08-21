using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.nflac.structure.metaheaders;

namespace org.nflac.audioformat.flac
{
    class FLACWaveHeader : WaveHeader
    {
        private StreamInfo streamInfo;

        public StreamInfo StreamInfo
        {
            get { return streamInfo; }
            set { streamInfo = value; }
        }

        public override ulong TotalSamples
        {
            get
            {
                return streamInfo.TotalSamples;
            }
        }

        public override ushort BitsPerSample
        {
            get
            {
                return streamInfo.BitsPerSample;
            }
        }

        public override ushort NumberOfChannels
        {
            get
            {
                return streamInfo.NumberOfChannels;
            }
        }

        public override uint SampleRate
        {
            get
            {
                return streamInfo.SampleRate;
            }
        }

        public FLACWaveHeader()
        {
        }

        public FLACWaveHeader(StreamInfo strInfo)
        {
            streamInfo = strInfo;
        }
    }
}
