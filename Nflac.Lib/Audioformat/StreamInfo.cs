using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Org.Nflac.Audioformat
{
    public class StreamInfo
    {
        public virtual ushort BitsPerSample { get; set; }
        public virtual ushort NumberOfChannels { get; set; }
        public virtual uint SampleRate { get; set; }
        public virtual ulong TotalSamples { get; set; }

        //public void Serialize(Stream stream);
        //public void Parse(Stream stream);
    }
}
