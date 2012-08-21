using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using org.nflac.structure;
using org.nflac.structure.util;

namespace org.nflac.audioformat
{
    public class WaveStream : Stream
    {
        private long position = 0;

        private WaveHeader waveHdr;
        private IPCMStream pcmStr;

        private int[] sample;

        private int bytesConsumed;
        private int bufferSize;

        byte[] header = new byte[WaveHeader.WAV_HEADER_SIZE];

        private byte[] outputBuffer;

        public WaveStream(WaveHeader hdr, IPCMStream pcmStream)
        {
            waveHdr = hdr;
            pcmStr = pcmStream;

            sample = new int[hdr.NumberOfChannels];
            bufferSize = hdr.NumberOfChannels*hdr.BitsPerSample/8;
            outputBuffer = new byte[bufferSize];
            
            MemoryStream hdrStream = new MemoryStream();
            hdr.WriteRIFFHeader(hdrStream);

            int i=0,b;
            hdrStream.Position = 0;

            while((b = hdrStream.ReadByte())!=-1){
                header[i++] = (byte)b;
            }

            pcmStr.Open();
            //so that the first byte is always read
            bytesConsumed = bufferSize;
        }

        private void ReadSample()
        {
            
            pcmStr.ReadSample(sample);

            int byteNum = 0;
            int sampleSplit = 0;

            for (int i = 0; i < waveHdr.NumberOfChannels; i++)
            {
                sampleSplit = sample[i];
                //TODO: write adequate conversion (queued/buffered again), write BitWriter.Flush()
                
                for (int j = 0; j < waveHdr.BitsPerSample / 8; j++)
                {
                    outputBuffer[byteNum++] = (byte) (sampleSplit & 0xFF);
                    sampleSplit >>= 8;
                }
            }

        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { return waveHdr.StreamSize; }
        }

        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        private int CopyBuffer(byte[] buffer, int offset, int count)
        {
            int bytesLeft = bufferSize - bytesConsumed;
            int bytesRead = 0;

            if (position >= WaveHeader.WAV_HEADER_SIZE)
            {
                for (int i = bytesConsumed; (i < bufferSize) && (bytesRead < count); i++)
                {
                    buffer[offset + bytesRead] = outputBuffer[i];
                    bytesRead++;
                    position++;
                }
            }
            else
            {
                for (int i = bytesConsumed; (i < bufferSize) && (bytesRead < count) && (position < WaveHeader.WAV_HEADER_SIZE); i++)
                {
                    buffer[offset + bytesRead] = outputBuffer[i];
                    bytesRead++;
                    position++;
                }
            }

            return bytesRead;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                int ret = 0, temp;

                int bytesLeft = count;

                if (position < WaveHeader.WAV_HEADER_SIZE)
                {
                    int readCnt = ((position + (long)bytesLeft) <= (long)WaveHeader.WAV_HEADER_SIZE) ? bytesLeft : (int)((long)WaveHeader.WAV_HEADER_SIZE - position - 1); 

                    ret = CopyBuffer(buffer, offset, readCnt);
                    bytesConsumed += ret;

                    bytesLeft -= ret;

                    if (position == WaveHeader.WAV_HEADER_SIZE)
                    {
                        bytesConsumed = bufferSize;
                    }

                    while (ret < count)
                    {
                        if (position < WaveHeader.WAV_HEADER_SIZE)
                        {
                            bytesConsumed = 0;

                            ReadHeader();

                            readCnt = ((position + (long)bytesLeft) < (long)WaveHeader.WAV_HEADER_SIZE) ? bytesLeft : (int)((long)WaveHeader.WAV_HEADER_SIZE - position - 1);

                            temp = CopyBuffer(buffer, offset + ret, bytesLeft);

                            bytesConsumed += temp;
                            bytesLeft -= temp;
                            ret += temp;
                        }
                        else
                        {
                            bytesConsumed = bufferSize; //ensure the actual stream is read
                            break;
                        }
                        
                        
                    }
                }

                if ((ret<count)&&(position >= WaveHeader.WAV_HEADER_SIZE))
                {
                    ret = CopyBuffer(buffer, offset, bytesLeft);
                    bytesConsumed += ret;

                    bytesLeft -= ret;

                    while (ret < count)
                    {
                        bytesConsumed = 0;

                        if (position < WaveHeader.WAV_HEADER_SIZE)
                        {
                            ReadHeader();
                        }
                        else
                        {
                            ReadSample();
                        }
                        temp = CopyBuffer(buffer, offset + ret, bytesLeft);

                        bytesConsumed += temp;
                        bytesLeft -= temp;
                        ret += temp;
                    }

                }

                return ret;
            }
            catch (EndOfStreamException ex)
            {
                //TODO: valid handling: close PCM stream. flush header stream
                return 0;
            }
        }

        private int ReadHeader()
        {
            int bytesRead = 0;

            for (int i = (int)position; (i < WaveHeader.WAV_HEADER_SIZE) && (bytesRead < bufferSize); i++)
            {
                outputBuffer[bytesRead++] = header[i];
            }

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
