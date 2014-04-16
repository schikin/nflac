using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Org.Nflac.Audioformat
{
    public abstract class AudioStream : Stream
    {        
        /// <summary>
        /// Pointer to current sample inside stream
        /// </summary>
        public abstract long SamplePosition { get; set; }

        public abstract void SeekSample(long sample);
    }
}
