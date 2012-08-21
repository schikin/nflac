using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders
{
    class SeekPoint
    {
        private ulong number;
        private ulong offset;

        private ushort numberOfSamples;

        public ulong Offset
        {
            get { return offset; }
        }

        public ulong Number
        {
            get { return number; }
        }

        public ushort NumberOfSamples
        {
            get { return numberOfSamples; }
        }

        public SeekPoint(ulong number, ulong offset, ushort numSamples)
        {
            this.number = number;
            this.offset = offset;
            this.numberOfSamples = numSamples;
        }

        public override string ToString()
        {
            return "Seekpoint " + number + " offset: " + offset + " samples: " + numberOfSamples;
        }
    }
}
