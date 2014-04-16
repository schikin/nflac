using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Org.Nflac.Audioformat
{
    public abstract class AudioEncoder
    {
        protected StreamInfo streamInfo;
        
        /// <summary>
        /// Information about audio stream
        /// </summary>
        public StreamInfo StreamInfo 
        {
            get
            {
                return streamInfo;
            }
        }

        protected Stream physicalStream;

        protected AudioDecoder decoder;

        public Stream PhysicalStream
        {
            get
            {
                return physicalStream;
            }
        }

        /// <summary>
        /// Write header to current physical stream position
        /// </summary>
        public abstract void WriterHeader();

        /*/// <summary>
        /// Write the number of samples from buffer
        /// </summary>
        /// <param name="sourceStreamInfo">Source stream metadata. Might be useful for transcoding</param>
        /// <param name="buffer">Source buffer</param>
        /// <param name="bitStart">Start position in source buffer (in bits)/// </param>
        /// <param name="count">Amount of samples to write</param>
        /// <returns>Number of samples successfully written or -1 if EOS ocurred.</returns>
        public long WriteSamples(StreamInfo sourceStreamInfo, byte[] buffer, long bitStart, long count); */

        /// <summary>
        /// Perform 1 discrete encoding step
        /// </summary>
        /// <returns>Number of samples encoded, -1 if EOS reached</returns>
        public abstract int EncodeStep(byte[] buffer, int start, int count);

        /// <summary>
        /// Convert StreamInfo to internal format
        /// </summary>
        /// <param name="info">Source stream info</param>
        protected abstract void ImportStreamInfo(StreamInfo info);

        /// <summary>
        /// Construct new encoder
        /// </summary>
        /// <param name="decoder">Source of audio data</param>
        /// <param name="outputStream">Output</param>
        /// <param name="recodeInfo">Information for new stream in reencode required. Otherwise kept as close as possible to source</param>
        public AudioEncoder(AudioDecoder decoder, Stream outputStream, StreamInfo recodeInfo)
        {
            this.physicalStream = outputStream;
            this.decoder = decoder;

            if (recodeInfo != null)
            {
                ImportStreamInfo(recodeInfo);
            }
            else
            {
                ImportStreamInfo(decoder.StreamInfo);
            }

            //WriterHeader();
        }

        public abstract void Flush();

        public abstract void Close();
    }
}
