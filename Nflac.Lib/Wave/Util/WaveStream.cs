using Org.Nflac.Audioformat;
using Org.Nflac.Wave.Exception;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Org.Nflac.Wave.Util
{
    public class WaveStream : AudioStream
    {
        private long currentPosition = 0;
        private long currentSample = 0;

        private long bufferHead = 0;
        private long bufferTail = 0;

        private byte[] dataBuffer;
        private static readonly int DATA_BUFFER_SIZE = 2048;

        private AudioDecoder decoder;
        private WaveEncoder encoder;

        private long CalculateSampleFromPosition(long position)
        {
           return (position < WaveStreamInfo.WAV_HEADER_SIZE) ? 0 : (position - WaveStreamInfo.WAV_HEADER_SIZE) / encoder.PhysicalBytesPerSample / encoder.StreamInfo.NumberOfChannels;
        }

        private long CalculatePositionFromSample(long sample)
        {
            return sample * encoder.PhysicalBytesPerSample * encoder.StreamInfo.NumberOfChannels + WaveStreamInfo.WAV_HEADER_SIZE;
        }

        public StreamInfo StreamInfo
        {
            get
            {
                return encoder.StreamInfo;
            }
        }

        public WaveStream(AudioDecoder decoder)
        {
            this.decoder = decoder;
            this.encoder = new WaveEncoder(decoder, new MemoryStream(), decoder.StreamInfo);
            dataBuffer = new byte[DATA_BUFFER_SIZE];
            bufferHead = 0;
            bufferTail = 0;
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

        private long CalculateWavLength()
        {
            uint dataSize = (uint)Math.Ceiling(((double)decoder.StreamInfo.TotalSamples * ((double)decoder.StreamInfo.BitsPerSample / 8d) * (double)(decoder.StreamInfo.NumberOfChannels)));

            return dataSize + WaveStreamInfo.WAV_HEADER_SIZE;
        }

        public override long Length
        {
            get { return CalculateWavLength(); }
        }

        public override long Position
        {
            get
            {
                return currentPosition;
            }
            set
            {
                SeekToPosition(value);
            }
        }

        private void SeekToPosition(long newPos)
        {
            if (newPos > Length)
            {
                throw new InvalidPositionException("Position outside of stream");
            }

            if (newPos < 0)
            {
                throw new InvalidPositionException("Position cannot be negative");
            }

            long oldPosition = currentPosition;
            currentPosition = newPos;            

            if (currentPosition < WaveStreamInfo.WAV_HEADER_SIZE)
            {
                bufferHead = 0;
                bufferTail = 0;
            }
            else
            {
                long absoluteBufferStart = oldPosition - bufferHead;
                long absoluteBufferEnd = absoluteBufferStart + bufferTail;

                if ((newPos <= absoluteBufferEnd) && (newPos >= absoluteBufferStart))
                {
                    bufferHead = newPos - absoluteBufferStart;
                }
                else
                {
                    var sampleRequested = CalculateSampleFromPosition(newPos);
                    var sampleAbsoluteStart = CalculatePositionFromSample(sampleRequested);
                    var inSampleShift = newPos - sampleAbsoluteStart;

                    decoder.Seek(sampleRequested);

                    RefillBuffer();

                    bufferHead = inSampleShift;
                    currentSample = sampleRequested;
                }
            }
        }

        private int RefillBuffer()
        {
            var estimateNumberOfSamples = DATA_BUFFER_SIZE / encoder.PhysicalBytesPerSample / encoder.StreamInfo.NumberOfChannels;

            int numSamples = encoder.EncodeStep(dataBuffer, 0, estimateNumberOfSamples);

            if (numSamples == -1)
            {
                return -1;
            }
            else
            {
                bufferHead = 0;
                bufferTail = numSamples * encoder.PhysicalBytesPerSample * encoder.StreamInfo.NumberOfChannels;

                return numSamples;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (currentPosition >= Length)
            {
                return 0;
            }
            
            int headerReadSize = 0;
            int dataReadSize = 0;

            byte[] header = null;

            if (currentPosition < WaveStreamInfo.WAV_HEADER_SIZE)
            {
                header = ((WaveStreamInfo)encoder.StreamInfo).GetRIFFHeader();

                var headerLeftSize = header.Length - currentPosition;

                if (headerLeftSize > count)
                {
                    headerReadSize = count;
                }
                else
                {
                    headerReadSize = (int)headerLeftSize; //potentially buggy casting, still header size is unlikely to overflow int
                }
            }

            int retSize = 0;
            dataReadSize = count - headerReadSize;

            int targetOffset = offset;

            if(headerReadSize > 0) 
            {
                //var headerBuffer = new byte[headerReadSize];

                //encoder.headerStream.Read(headerBuffer, 0, headerReadSize);

                Array.Copy(header, currentPosition, buffer, targetOffset, headerReadSize);
                retSize += headerReadSize;
                targetOffset += headerReadSize;
                currentPosition += headerReadSize;
            }

            if (dataReadSize > 0)
            {
                //var estimateNumberOfSamples = dataReadSize / encoder.PhysicalBytesPerSample / encoder.StreamInfo.NumberOfChannels;

                //if (estimateNumberOfSamples == 0)
                //{
                //    estimateNumberOfSamples = 1;
                //}
                long dataLeft = dataReadSize;

                for (; ; )
                {
                    var estimateNumberOfSamples = DATA_BUFFER_SIZE / encoder.PhysicalBytesPerSample / encoder.StreamInfo.NumberOfChannels;

                    long bytesInBuffer = bufferTail - bufferHead;
                    long bytesFromBuffer = dataLeft;

                    if (dataLeft > bytesInBuffer)
                    {
                        bytesFromBuffer = bytesInBuffer;
                    }

                    if (bytesFromBuffer > 0)
                    {
                        Array.Copy(dataBuffer, bufferHead, buffer, targetOffset, bytesFromBuffer);
                        currentPosition += bytesFromBuffer;
                        targetOffset += (int)bytesFromBuffer;
                        retSize += (int)bytesFromBuffer;
                        dataLeft -= bytesFromBuffer;
                    }

                    bufferHead += bytesFromBuffer;

                    //long bytesFromNextFrame = dataReadSize - bytesFromBuffer;
                    int numSamples;

                    if (bufferHead == bufferTail)
                    {
                        numSamples = RefillBuffer();

                        if (numSamples == -1)
                        {
                            /* if (retSize == 0)
                            {
                                return 0;
                            }
                            else
                            {
                                return retSize;
                            } */
                            return retSize;
                        }
                        
                    }

                    if (retSize == count)
                    {
                        return retSize;
                    }

                    //var dataBuffer = new byte[dataReadSize];

                    //var dataStreamPosition = currentPosition - encoder.headerStream.Length;

                    //encoder.dataStream.Position = ((dataStreamPosition > 0) ? dataStreamPosition : 0);
                    //if (bytesFromNextFrame > 0)
                    //{

                    //}
                }


            }
            
            return retSize;
        }

        public override long SamplePosition
        {
            get
            {
                return CalculateSampleFromPosition(currentPosition);
            }

            set
            {
                SeekSample(value);
            }
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

        public override void SeekSample(long sample)
        {
            SeekToPosition(CalculatePositionFromSample(sample));
        }
    }
}
