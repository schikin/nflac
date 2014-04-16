using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Org.Nflac.Audioformat
{
    public abstract class AudioDecoder
    {
        protected StreamInfo streamInfo;

        public StreamInfo StreamInfo
        {
            get
            {
                return streamInfo;
            }
        }

        protected Stream physicalStream;

        public Stream PhysicalStream
        {
            get
            {
                return physicalStream;
            }
        }

        /// <summary>
        /// Read the number of samples into buffer
        /// </summary>
        /// <param name="buffer">Output buffer to store samples. Must be of size StreamInfo.SampleSize * count</param>
        /// <param name="start">Start position at output buffer</param>
        /// <param name="count">Number of samples to read</param>
        /// <returns>Number of samples successfully read or -1 if EOS reached</returns>
        public abstract int ReadSample(byte[] buffer, int start, int count);

        /// <summary>
        /// Read the header from file
        /// </summary>
        protected abstract void ReadHeader();

        /// <summary>
        /// Seek to position
        /// </summary>
        /// <param name="sample">Number of sample to set pointer to</param>
        public abstract void Seek(long sample);

        public AudioDecoder(Stream source)
        {
            this.physicalStream = source;

            ReadHeader();
        }
    }
}
