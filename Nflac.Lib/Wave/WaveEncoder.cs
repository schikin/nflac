using Org.Nflac.Audioformat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Org.Nflac.Wave
{
    public class WaveEncoder : AudioEncoder
    {
        internal MemoryStream headerStream;
        internal MemoryStream dataStream;

        private readonly int sampleBufferSize = 2048;

        public WaveEncoder(AudioDecoder decoder, Stream outputStream, StreamInfo recodeInfo)
            : base(decoder, outputStream, recodeInfo)
        {

        }

        protected override void ImportStreamInfo(StreamInfo info)
        {
            var newInfo = new WaveStreamInfo();

            newInfo.BitsPerSample = info.BitsPerSample;
            newInfo.NumberOfChannels = info.NumberOfChannels;
            newInfo.SampleRate = info.SampleRate;
            newInfo.TotalSamples = info.TotalSamples;

            newInfo.AudioFormat = WaveCompression.PCM;

            this.streamInfo = newInfo;
        }

        public override void WriterHeader()
        {
            var wavStreamInfo = (WaveStreamInfo)streamInfo;

            wavStreamInfo.WriteRIFFHeader(headerStream);
        }

        public int PhysicalBytesPerSample
        {
            get
            {
                return (streamInfo.BitsPerSample % 8 == 0) ? streamInfo.BitsPerSample / 8 : streamInfo.BitsPerSample / 8 + 1;
            }
        }

        private byte[] StuffSamples(byte[] data, int numChannels, int bitsFrom, int numSamples)
        {
            if (bitsFrom % 8 == 0)
            {
                return data;
            }

            //int newBitCount = (bitsFrom % 8 == 0) ? bitsFrom / 8 : bitsFrom / 8 + 1;
            int newByteCount = bitsFrom / 8 + 1;
            byte bitshift = (byte)(8 - bitsFrom % 8);

            byte[] ret = new byte[numSamples * (newByteCount) * numChannels];

            long totalBitsInChannel = numSamples * bitsFrom;

            int currentByteBitPosition = 8;
            long currentSourceByteNum = 0;
            byte currentSourceByte = 0;

            long currentTargetByteNum = 0;

            for (int i = 0; i < numChannels; i++)
            {
                for (int j = 0; j < numSamples; j++)
                {
                    int bitsInSampleLeft = bitsFrom;
                    int sampleByteNumber = 1;

                    while (bitsInSampleLeft > 0)
                    {
                        int bitsInSampleToRead = bitsInSampleLeft % 8;

                        int bitsLeft = 8 - currentByteBitPosition;

                        if (bitsLeft == 0)
                        {
                            currentSourceByte = data[currentSourceByteNum++];
                            bitsLeft = 8;
                        }

                        byte currentSample = 0;

                        if (bitsLeft > bitsInSampleToRead)
                        {
                            currentSample = (byte)(((currentSourceByte << currentByteBitPosition) >> (8 - currentByteBitPosition)) >> (8 - bitsInSampleToRead));
                            currentByteBitPosition += bitsInSampleToRead;
                        }
                        else
                        {
                            int bitsFromNextByte = bitsLeft - bitsInSampleToRead;

                            byte currentSampleFirstByte = (byte)((currentSourceByte << currentByteBitPosition) >> (8 - currentByteBitPosition));

                            currentSourceByte = data[currentSourceByteNum++];

                            byte currentSampleSecondByte = (byte)(currentSourceByte >> (8 - bitsFromNextByte));

                            currentSample = (byte)((currentSampleFirstByte << bitsFromNextByte) | currentSampleSecondByte);

                            currentByteBitPosition = bitsFromNextByte;
                        }

                        bitsInSampleLeft -= bitsInSampleToRead;

                        if (sampleByteNumber++ == newByteCount)
                        {
                            ret[currentTargetByteNum++] = (byte)(currentSample << bitshift);
                        }
                        else
                        {
                            ret[currentTargetByteNum++] = currentSample;
                        }
                    }
                }
            }

            return ret;
        }

        public override int EncodeStep(byte[] buffer, int start, int count)
        {
            var readBuffer = new byte[sampleBufferSize * (streamInfo.BitsPerSample / 8 + 1) * streamInfo.NumberOfChannels];

            int numSamples = decoder.ReadSample(readBuffer, 0, count);

            if (numSamples == -1)
            {
                return -1;
            }

            byte[] intermediateBuffer = null;

            if (decoder.StreamInfo.BitsPerSample % 8 != 0)
            {
                intermediateBuffer = StuffSamples(readBuffer, (int)decoder.StreamInfo.NumberOfChannels, (int)decoder.StreamInfo.BitsPerSample, (int)decoder.StreamInfo.TotalSamples);
            }
            else
            {
                intermediateBuffer = readBuffer;
            }

            int bytesPerSample = (decoder.StreamInfo.BitsPerSample % 8 == 0) ? decoder.StreamInfo.BitsPerSample / 8 : decoder.StreamInfo.BitsPerSample / 8 + 1;

            for (int i = 0; i < numSamples; i++)
            {
                for (int channelNum = 0; channelNum < streamInfo.NumberOfChannels; channelNum++)
                {
                    for (int j = 0; j < bytesPerSample; j++)
                    {

                        //dataStream.Write(intermediateBuffer,i * channelNum + j,1); 
                        buffer[start + i * bytesPerSample * streamInfo.NumberOfChannels + bytesPerSample * channelNum + j] =
                            //intermediateBuffer[channelNum * bytesPerSample * numSamples + j];
                            intermediateBuffer[i * bytesPerSample * streamInfo.NumberOfChannels + bytesPerSample * channelNum + (bytesPerSample - j - 1)];
                    }
                }
            }

            return numSamples;
        }

        public override void Flush()
        {
            WriterHeader();

            headerStream.Position = 0;
            dataStream.Position = 0;

            headerStream.CopyTo(physicalStream);
            dataStream.CopyTo(physicalStream);

            physicalStream.Flush();
        }

        public override void Close()
        {
            Flush();

            PhysicalStream.Close();
        }
    }
}
