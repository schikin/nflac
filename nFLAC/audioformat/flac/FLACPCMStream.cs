using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.nflac.structure.data;
using System.IO;
using System.Threading;
using org.nflac.structure.metaheaders;
using org.nflac.structure.exceptions;

namespace org.nflac.audioformat.flac
{
    public class FLACPCMStream : IPCMStream
    {
        private int frameBufferSize;

        public int FrameBufferSize
        {
            get { return frameBufferSize; }
            set { frameBufferSize = value; }
        }

        private Queue<Frame> cache;
        private Queue<Frame> refillQueue;

        private Thread refiller;
        private AutoResetEvent needRefillEvent;
        private AutoResetEvent frameReadEvent;
        private AutoResetEvent threadStarted;

        private int samplesConsumed;
        private Frame outputBuffer;

        private StreamInfo streamInfo;

        private Stream inputStream;

        private bool isOpen = false;
        private bool isWaiting = false;
        private bool endOfStream = false;

        public FLACPCMStream()
        {
            Initialize();
            StartRefiller();
        } 

        public FLACPCMStream(Stream input, StreamInfo strInfo)
        {
            Initialize();
            streamInfo = strInfo;
            inputStream = input;
            StartRefiller();
        }

        private void Initialize()
        {
            needRefillEvent = new AutoResetEvent(false);
            frameReadEvent = new AutoResetEvent(false);
            outputBuffer = new Frame();
            this.FrameBufferSize = 3; //default value
            cache = new Queue<Frame>();
            refillQueue = new Queue<Frame>();
        }

        private void StartRefiller()
        {
            threadStarted = new AutoResetEvent(false);

            refiller = new Thread(new ThreadStart(Refill));

            refiller.Start();

            threadStarted.WaitOne();
        }

        private void Refill()
        {            
            threadStarted.Set();

            Queue<Frame> toRefill = new Queue<Frame>();
            Frame fr;

            while(true)
            {
                needRefillEvent.WaitOne();

                lock (refillQueue)
                {
                    toRefill.Enqueue(refillQueue.Dequeue());
                }

                while (toRefill.Count > 0)
                {                    

                    fr = toRefill.Dequeue();

                    lock (inputStream)
                    {
                        try
                        {
                            fr.Decode(inputStream, streamInfo);
                        }
                        catch (EndOfStreamException ex)
                        {
                            endOfStream = true;
                        }
                        //Console.WriteLine("[async] {0}", fr);
                    }

                    if (!endOfStream)
                    {
                        lock (cache)
                        {
                            cache.Enqueue(fr);
                        }
                    }
                                
                    frameReadEvent.Set();
                }
            }
        }

        public void Open()
        {
            UpdateCache();
            isOpen = true;
        }

        public void Close()
        {
            isOpen = false;
        }

        public void ReadSample(int[] buffer)
        {
            if (!isOpen)
            {
                throw new PCMStreamNotOpen();
            }

            for (int i = 0; i < outputBuffer.Channels; i++)
            {
                buffer[i] = outputBuffer.output[i, samplesConsumed];
            }

            if (++samplesConsumed >= (int)outputBuffer.Blocksize)
            {
                UpdateCache();
            }

        }

        private void UpdateCache()
        {
            samplesConsumed = 0;
            bool needRefill = false;
            bool waitForRead = false;

            lock (cache)
            {
                needRefill = cache.Count < frameBufferSize;
            }

            if (needRefill)
            {
                lock (refillQueue)
                {
                    refillQueue.Enqueue(outputBuffer);
                }

                needRefillEvent.Set();
            }

            lock (cache)
            {
                waitForRead = cache.Count == 0;
            }

            if (waitForRead)
            {
                frameReadEvent.WaitOne();
            }

            if (endOfStream)
            {
                throw new EndOfStreamException();
            }

            lock (cache)
            {
                outputBuffer = cache.Dequeue();
                Console.WriteLine(outputBuffer);
                if (!waitForRead)
                {
                    frameReadEvent.WaitOne();
                }
            }
        }
    }
}
