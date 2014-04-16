using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Audioformat
{
    /// <summary>
    /// Container for audio data in internal format.
    /// </summary>
    public class MultiplexedFrame
    {
        public byte[,] Buffer { get; set; }
        public ulong SampleCount { get; set; }
        public long BufferUsed { get; set; }
        public int BitOffset { get; set; }

        public int BitsInSample { get; set; }
        public int NumChannels { get; set; }

        public void ToByteArray(byte[] buffer, int start)
        {
            int targetPos = start;
            int targetBitOffset = 0;

            int cBitOffset = 0;
            int cByte = 0;

            for (ulong sampleNum = 0; sampleNum < SampleCount; sampleNum++)
            {
                int newBytePos = 0;

                for (int chan = 0; chan < NumChannels; chan++)
                {                    
                    int bitsLeft = BitsInSample;
                    int byteShift = 0;
                    
                    while(bitsLeft > 0)
                    {
                        int bitsToRead = 0;

                        if(bitsLeft > 8 - targetBitOffset)
                        {
                            bitsToRead = 8 - targetBitOffset;
                        }
                        else
                        {
                            bitsToRead = bitsLeft;
                        }

                        if(bitsToRead > 8 - cBitOffset)
                        {
                            bitsToRead = 8 - cBitOffset;
                        }
                        
                        byte sampleChunk = (byte) ((Buffer[chan,cByte+byteShift] << cBitOffset) >> (8-bitsToRead));

                        buffer[targetPos] |= (byte)((sampleChunk << (8-bitsToRead)) >> (targetBitOffset));

                        targetBitOffset += bitsToRead;
                        cBitOffset += bitsToRead;
                        bitsLeft -= bitsToRead;

                        if(targetBitOffset == 8)
                        {
                            targetBitOffset = 0;
                            targetPos++;
                        }

                        if(cBitOffset == 8)
                        {
                            cBitOffset = 0;

                            newBytePos = ++byteShift;
                        }
                    }
                }
   
                cByte+=newBytePos;
            }
            
        }

    }
}
