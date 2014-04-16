using Org.Nflac.Audioformat;
using Org.Nflac.Data;
using Org.Nflac.Flac.Exceptions;
using Org.Nflac.Flac.Metaheaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Integration
{
    public class FlacDecoder : AudioDecoder
    {
        private Org.Nflac.Flac.Metaheaders.StreamInfo flacStreamInfo;

        private const ulong FLAC_HEADER = 0x664C6143; //fLaC in ASCII

        private List<Metadata> headers = new List<Metadata>();

        private ulong currentSample;

        private Frame currentFrame = new Frame();

        public bool AutoSyncronize { get; set; }

        private bool frameDecoded = false;

        private long startOfAudioStream = 0;

        public FlacDecoder(Stream stream)
            : base(stream)
        {
            //ReadHeader();
        }

        public void SynchronizeRight()
        {
            currentFrame.SyncronizeRight(physicalStream, flacStreamInfo);

            frameDecoded = false;
        }

        public void SynchronizeLeft()
        {
            currentFrame.SyncronizeLeft(physicalStream, flacStreamInfo);

            frameDecoded = false;
        }

        private void MultiplexFrame(Frame fr, MultiplexedFrame mfr, int numSamples, ulong fromSample)
        {
            long startBuffer = mfr.BufferUsed;

            for (int chan = 0; chan < mfr.NumChannels; chan++)
            {
                mfr.BufferUsed = startBuffer;

                for (int sampleNum = 0; sampleNum < numSamples; sampleNum++)
                {
                    int sampleToMultiplex = fr.output[chan, sampleNum+(int)(fromSample-fr.FirstSampleNumber)];

                    int bitsLeft = streamInfo.BitsPerSample;
                    int bitsRead = 0;

                    while (bitsLeft > 0)
                    {
                        int bitsToAdd = 0;

                        if (bitsLeft > 8 - mfr.BitOffset)
                        {
                            bitsToAdd = 8 - mfr.BitOffset;
                        }
                        else
                        {
                            bitsToAdd = bitsLeft;
                        }

                        mfr.Buffer[chan, mfr.BufferUsed] |= //(byte)(((byte)(sampleToMultiplex << bitsRead)) >> (8 - bitsToAdd));
                            (byte)(sampleToMultiplex >> streamInfo.BitsPerSample - bitsRead - bitsToAdd);

                        bitsRead += bitsToAdd;
                        mfr.BitOffset += bitsToAdd;
                        bitsLeft -= bitsToAdd;

                        if (mfr.BitOffset == 8)
                        {
                            mfr.BufferUsed++;
                            mfr.BitOffset = 0;
                        }
                    }
                }
            }

            mfr.SampleCount += (ulong)numSamples;
        }

        public override int ReadSample(byte[] buffer, int start, int count)
        {
            MultiplexedFrame fr = new MultiplexedFrame();

            fr.BitsInSample = streamInfo.BitsPerSample;
            fr.NumChannels = streamInfo.NumberOfChannels;
            fr.SampleCount = 0;
            fr.BitOffset = 0;
            fr.BufferUsed = 0;
            fr.Buffer = new byte[fr.NumChannels,count * (fr.BitsInSample / 8 + 1)];

            int sampleCount = 0;
            int samplesLeft = count;

            try
            {
                for (; ; )
                {
                    if(!frameDecoded)
                    {
                        currentFrame.Decode(physicalStream,flacStreamInfo);
                        frameDecoded = true;
                    }

                    int samplesToRead = 0;

                    if((ulong)samplesLeft > currentFrame.LastSampleNumber - currentSample + 1) 
                    {
                        samplesToRead = (int) (currentFrame.LastSampleNumber - currentSample + 1);
                    }
                    else 
                    {
                        samplesToRead = samplesLeft;
                    }

                    MultiplexFrame(currentFrame,fr,samplesToRead,currentSample);

                    samplesLeft -= samplesToRead;
                    currentSample += (ulong)samplesToRead;
                    sampleCount += samplesToRead;

                    if (samplesLeft > 0)
                    {
                        currentFrame.Decode(physicalStream, flacStreamInfo);
                        frameDecoded = true;
                    }

                    if (sampleCount == count)
                    {
                        break;
                    }
                }
            }
            catch (LostSynchronizationException)
            {
                if (AutoSyncronize)
                {
                    SynchronizeRight();
                }
            }
            catch (MalformedFileException)
            {
                if (AutoSyncronize)
                {
                    SynchronizeRight();
                }
            }
            catch (UnexpectedEndOfStreamException)
            {
                if (sampleCount == 0)
                {
                    sampleCount = -1;
                }
            }

            if (sampleCount != -1)
            {
                fr.ToByteArray(buffer, start);
            }

            return sampleCount;
        }

        protected override void ReadHeader()
        {
            CheckFile();
            ParseHeaders();

            startOfAudioStream = physicalStream.Position;
            currentSample = 0;
        }

        private void CheckFile()
        {
            ulong header = 0;
            byte buff;

            int i;

            for (i = 0; i < 4; i++)
            {
                header = header << 8;
                buff = (byte)physicalStream.ReadByte();
                header |= buff;
            }

            if (header != FLAC_HEADER)
            {
                throw new FlacHeaderException("FLAC stream does not start with \"fLaC\"");
            }
        }

        private void ParseHeaders()
        {
            Metadata block = null;

            do
            {
                block = Metadata.Decode(physicalStream);

                if (block is Org.Nflac.Flac.Metaheaders.StreamInfo)
                {
                    flacStreamInfo = (Org.Nflac.Flac.Metaheaders.StreamInfo)block;
                    streamInfo = new FLACNFlacInfo(flacStreamInfo);
                }

                headers.Add(block);
            } while (block.IsLastBlock == false);
        }

        public override void Seek(long sample)
        {
            if ((ulong)sample > streamInfo.TotalSamples-1)
            {
                throw new InvalidSampleNumber(String.Format("Trying to seek to sample {0} where only {1} samples available in stream",sample,streamInfo.TotalSamples));
            }

            if(sample < 0)
            {
                throw new InvalidSampleNumber("Cannot seek to negative sample");
            }

            if (((ulong)sample >= currentFrame.FirstSampleNumber) && ((ulong)sample <= currentFrame.LastSampleNumber))
            {
                currentSample = (ulong)sample;
            }
            else
            {

                //1. find nearest preceding seekpoint
                SeekPoint p = null;
                SeekTable tbl = null;

                foreach (var m in headers)
                {
                    if (m is SeekTable)
                    {
                        tbl = (SeekTable)m;
                    }
                }

                ulong minDistance = UInt64.MaxValue;

                if (tbl != null)
                {

                    foreach (var point in tbl.Points)
                    {
                        if (point.Number > (ulong)sample)
                        {
                            continue;
                        }

                        if (((ulong)sample - point.Number) < minDistance)
                        {
                            p = point;
                            minDistance = (ulong)sample - point.Number;
                        }
                    }
                }

                long startSeek = startOfAudioStream;

                if (p != null)
                {
                    startSeek = (long)p.Number;
                }

                //physicalStream.Position = startSeek;
                long left = startSeek;
                long right = physicalStream.Length;

                int dbgCtr = 0;

                for (; ; )
                {
                    if (dbgCtr++ == 10000)
                    {
                        //DBG break
                        Console.WriteLine("fail");
                    }

                    physicalStream.Position = (left + right) / 2;
                    long binSearchPos = physicalStream.Position;

                    try
                    {
                        SynchronizeRight();

                    }
                    catch (LostSynchronizationException)
                    {
                        SynchronizeLeft();
                    }

                    long frameStartPos = physicalStream.Position;

                    currentFrame.ParseHeader(physicalStream, flacStreamInfo);

                    if (((ulong)sample >= currentFrame.FirstSampleNumber) && ((ulong)sample <= currentFrame.LastSampleNumber))
                    {
                        physicalStream.Position = frameStartPos;

                        currentFrame.Decode(physicalStream, flacStreamInfo);
                        frameDecoded = true;

                        currentSample = (ulong)sample;
                        return;
                    }
                    else
                    {
                        if ((ulong)sample > currentFrame.LastSampleNumber)
                        {
                            left = binSearchPos;

                        }
                        else
                        {
                            right = binSearchPos;
                        }
                    }
                }
            }
            
        }
    }
}
