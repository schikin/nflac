using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Nflac.Audioformat;

namespace Org.Nflac.Flac.Integration
{
    class FLACNFlacInfo : StreamInfo
    {
        private Org.Nflac.Flac.Metaheaders.StreamInfo streamInfo;

        public Org.Nflac.Flac.Metaheaders.StreamInfo StreamInfo
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

        public FLACNFlacInfo()
        {
        }

        public FLACNFlacInfo(Org.Nflac.Flac.Metaheaders.StreamInfo strInfo)
        {
            streamInfo = strInfo;
        }
    }
}
