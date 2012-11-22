using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using org.nflac.structure.util;
using org.nflac.structure.metaheaders;
using org.nflac.structure.exceptions;
using org.nflac.structure.exceptions.stream;

namespace org.nflac.structure.data
{
    class Frame
    {
        public const int MAX_CHANNELS = 7;

        private int subFrameConst = 0;

        public int SubFrameConst
        {
            get { return subFrameConst; }
            set { subFrameConst = value; }
        }

        private int subFrameVerbatim = 0;

        public int SubFrameVerbatim
        {
            get { return subFrameVerbatim; }
            set { subFrameVerbatim = value; }
        }

        private int subFrameLPC = 0;

        public int SubFrameLPC
        {
            get { return subFrameLPC; }
            set { subFrameLPC = value; }
        }

        private int subFrameFixed = 0;

        public int SubFrameFixed
        {
            get { return subFrameFixed; }
            set { subFrameFixed = value; }
        }
        

        internal int[,] output = new int[7, Subframe.MAX_BLOCKSIZE];

        public int[,] Output
        {
            get { return output; }
        }

        private Boolean variableLength;

        public Boolean VariableLength
        {
            get { return variableLength; }
        }

        private ulong blocksize;

        public ulong Blocksize
        {
            get { return blocksize; }
        }

        private ulong sampleRate;

        public ulong SampleRate
        {
            get { return sampleRate; }
        }

        private byte channels;

        public byte Channels
        {
            get { return channels; }
        }

        private ChannelAssignment channelSetup;

        internal ChannelAssignment ChannelSetup
        {
            get { return channelSetup; }
        }

        private byte sampleSize;

        public byte SampleSize
        {
            get { return sampleSize; }
        }

        private ulong frameNumber;

        public ulong FrameNumber
        {
            get { return frameNumber; }
        }

        /// <summary>
        /// Reads the frame header from the stream.
        /// 
        /// Format description: http://flac.sourceforge.net/format.html#frame_header
        /// </summary>
        /// <param name="stream">The stream to read frame header from</param>
        public void Decode(Stream stream, StreamInfo streamInfo)
        {
            ushort buffer;

            int additionalBlockSize = 0; //number of additional bytes at the end of the header that represent its length

            int sampleRateBytes = 0; //number of additional bytes at the end of the header that store the sample rate
            int sampleRateMultiplicator = 1; //scale of the stored sample rate

            buffer = org.nflac.structure.util.StreamReader.ReadUShort(stream);

            byte[] rawHeader = new byte[16];
            byte headerLen = 2;

            rawHeader[0] = (byte) (buffer >> 8);
            rawHeader[1] = (byte) (buffer & 0xff);

            if ((buffer & 0xfff8) != 0xfff8)
            {
                throw new LostSynchronizationException();
                //TODO: desync
            }
            else
            {
                //Console.WriteLine("SYNC: " + (stream.Position-2));
            }

            if ((buffer & 0x4) != 0)
            {
                //TODO: reserved bit violation
                return;
            }

            variableLength = (buffer & 0x1) == 0x1;

            byte size;

            buffer = org.nflac.structure.util.StreamReader.ReadUShort(stream);

            rawHeader[headerLen++] = (byte) (buffer >> 8);
            rawHeader[headerLen++] = (byte) (buffer & 0xff);

            size = (byte) ((buffer & 0xF000) >> 12 ); //higher 4 bits

            if (size == 0x0)
            {
                //reserved block-size
                return;
            }
            else if (size == 0x1)
            {
                blocksize = 192;
            }
            else if ((size > 0x1) && (size < 0x6))
            {
                blocksize = (ulong)(576 << (size - 2));
            }
            else if (size == 0x6)
            {
                additionalBlockSize = 1; //get additional byte from the end of the header
            }
            else if (size == 0x7)
            {
                additionalBlockSize = 2; //get 2 bytes from the end of the header
            }
            else if ((size > 0x7) && (size < 0x11))
            {
                blocksize = (ulong)(256 << (size - 8));
            }
            else
            {
                //TODO: handle reserved blocksize value
                return;
            }

            byte rateCoded;

            rateCoded = (byte)((buffer & 0x0F00) >> 8); //next 4 bits
        
            switch(rateCoded)
            {
                case 0:
                    //get from STREAMINFO
                    break;
                case 1:
                    sampleRate = 88200;
                    break;
                case 2:
                    sampleRate = 176400;
                    break;
                case 3:
                    sampleRate = 192000;
                    break;
                case 4:
                    sampleRate = 8000;
                    break;
                case 5:
                    sampleRate = 16000;
                    break;
                case 6:
                    sampleRate = 22050;
                    break;
                case 7:
                    sampleRate = 24000;
                    break;
                case 8:
                    sampleRate = 32000;
                    break;
                case 9:
                    sampleRate = 44100;
                    break;
                case 10:
                    sampleRate = 48000;
                    break;
                case 11:
                    sampleRate = 96000;
                    break;
                case 12:
                    sampleRate = 0;
                    sampleRateBytes = 1;
                    sampleRateMultiplicator = 1000;
                    break;
                case 13:
                    sampleRate = 0;
                    sampleRateBytes = 2;
                    sampleRateMultiplicator = 1;
                    break;
                case 14:
                    sampleRate = 0;
                    sampleRateBytes = 2;
                    sampleRateMultiplicator = 10;
                    break;
                case 15:
                    //handle reserved sample rate value
                    return;
            }

            byte channelSetupCoded;

            channelSetupCoded = (byte)((buffer & 0x00F0) >> 4); //next 4 bits

            if (channelSetupCoded < 8)
            {
                if ( (channelSetupCoded == 6) || (channelSetupCoded == 7) )
                {
                    //some specific code for 7- and 8- channel music might be needed (those do not have appropriate channel assignment in the standard)
                }
                
                channels = (byte) (channelSetupCoded + 1);
                channelSetup = (ChannelAssignment) channelSetupCoded;
            }
            else if ((channelSetupCoded > 7) && (channelSetupCoded < 11))
            {
                //stereo
                channels = 2;
                channelSetup = (ChannelAssignment)channelSetupCoded;
            }
            else
            {
                //TODO: handle reserved channel assignment values
                return;
            }

            byte sampleSizeCoded;

            sampleSizeCoded = (byte)((buffer & 0x000E) >> 1); //next 3 bits

            switch (sampleSizeCoded)
            {
                case 0:
                    //get from STREAMINFO
                    break;
                case 1:
                    sampleSize = 8;
                    break;
                case 2:
                    sampleSize = 12;
                    break;
                case 3:
                    //reserved value 1 handling
                    return;
                case 4:
                    sampleSize = 16;
                    break;
                case 5:
                    sampleSize = 20;
                    break;
                case 6:
                    sampleSize = 24;
                    break;
                case 7:
                    //reserved value 2 handling
                    return;
            }

            byte mandatoryField2;

            mandatoryField2 = (byte)(buffer & 0x0001); //last 1 bit
        
            if (mandatoryField2 != 0)
            {
                //TODO: handle mandatory value 2 violation
                return;
            }

            //handle UTF-8-like number
            frameNumber = ReadUTF8Number(stream,rawHeader, ref headerLen);

            if (frameNumber == 32)
            {
                //breakpoint
            }

            //handle additional bytes for blocksize

            for (int i = 0; i < additionalBlockSize; i++)
            {
                blocksize = (ulong)(blocksize << 8);
                buffer = (ushort)stream.ReadByte();
                rawHeader[headerLen++]=(byte)buffer;
                blocksize |= buffer;
            }

            if (frameNumber * streamInfo.MaximumBlockSize + blocksize > streamInfo.TotalSamples)
            {
                blocksize = (streamInfo.TotalSamples - frameNumber * streamInfo.MaximumBlockSize);
            }

            //handle additional sample rate

            for (int i = 0; i < sampleRateBytes; i++)
            {
                sampleRate = (ulong)(sampleRate << 8);
                buffer = (ushort)stream.ReadByte();
                rawHeader[headerLen++]=(byte)buffer;
                sampleRate |= buffer;
            }

            sampleRate *= (ulong)sampleRateMultiplicator;

            byte crc;

            crc = (byte)stream.ReadByte();

            byte[] headerVal = new byte[headerLen];
            
            for(int i = 0; i<headerLen; i++)
            {
                headerVal[i] = rawHeader[i];
            }

            if (crc != CRC8.Instance.Checksum(headerVal))
            {
                //CRC-8 failed
                Console.WriteLine(frameNumber + " CRC-8 failed");
            }
            else
            {
                //Console.WriteLine(frameNumber + " CRC-8 passed");
            }

            Subframe subfr = new Subframe();
            BitReader br = new BitReader(stream);

            br.IsRecording = true;

            for (int i = 0; i < headerVal.Length; i++)
            {
                br.RememberStream.WriteByte(headerVal[i]);
            }

            br.RememberStream.WriteByte(crc);

            double timeStart, timeEnd;
            SubframeType ch1Type = SubframeType.LPC, ch2Type = SubframeType.LPC;

            timeStart = ((double)(frameNumber * blocksize)) / (double)sampleRate;
            timeEnd = timeStart + ((double)blocksize / (double)sampleRate);

            for (int i = 0; i < channels; i++)
            {
                int bitsPerSampleSubfr = sampleSize;

                switch (channelSetup)
                {
                    case ChannelAssignment.MID_SIDE:
                        if (i == 1)
                        {
                            bitsPerSampleSubfr++;
                        }
                        break;
                    case ChannelAssignment.LEFT_SIDE:
                        if (i == 1)
                        {
                            bitsPerSampleSubfr++;
                        }
                        break;
                    case ChannelAssignment.RIGHT_SIDE:
                        if (i == 0)
                        {
                            bitsPerSampleSubfr++;
                        }
                        break;
                }

                subfr.Decode(this, streamInfo, br, bitsPerSampleSubfr, i);
                if (i == 0)
                {
                    ch1Type = subfr.Type;
                }
                else if (i == 1)
                {
                    ch2Type = subfr.Type;
                }

                //Console.WriteLine(subfr);
                switch(subfr.Type)
                {
                    case SubframeType.Constant:
                        subFrameConst++;
                        break;
                    case SubframeType.Verbatim:
                        subFrameVerbatim++;
                        break;
                    case SubframeType.LPC:
                        subFrameLPC++;
                        break;
                    case SubframeType.Fixed:
                        subFrameFixed++;
                        break;
                }

                
            }

            //Console.WriteLine("[{0:00.0000}-{1:00.0000}] setup: {2} 1ch: {3}, 2ch: {4}", timeStart, timeEnd, channelSetup, ch1Type, ch2Type);


            //decode channels if needed
            switch (channelSetup)
            {
                case ChannelAssignment.MID_SIDE:
                    for (ulong i = 0; i < blocksize; i++)
                    {
                        int mid = (int)output[0,i];
 	                    int side = (int) output[1,i];
 	                    mid <<= 1;
 	                    mid |= (side & 1); 
 	                    output[0,i] = (mid + side) >> 1;
 	                    output[1,i] = (mid - side) >> 1;
                    }
                    break;
                case ChannelAssignment.LEFT_SIDE:
                    for (ulong i = 0; i < blocksize; i++)
                    {
                        if (i == 264)
                        {
                            //breakpoint
                        }
                        output[1, i] = output[0, i] - output[1, i];
                    }
                        
                    
                    break;
                case ChannelAssignment.RIGHT_SIDE:
                    for (ulong i = 0; i < blocksize; i++)
                    {
                        output[0, i] = output[1, i] + output[0, i];
                    }
                    break;

            }

            br.IsRecording = false;

            ushort crc16 = (ushort)org.nflac.structure.util.StreamReader.ReadUShort(stream);

            ushort crc16_calc = CRC16.Instance.Checksum(br.RememberStream);

            if (crc16 == crc16_calc)
            {
                //CRC-16 failed
                Console.WriteLine(frameNumber + " CRC-16 failed");
            }
            else
            {
                //Console.WriteLine(frameNumber + " CRC-16 passed");
            }
        }

        //TODO: Сделать второй метод, для разбора короткого (5-ти байтового числа). Он будет отличаться - предпоследний бит может быть значащим!
        private ulong ReadUTF8Number(Stream str, byte[] headerVal, ref byte headerLen)
        {
            ulong v = 0;
            byte x;
            int i;

            x = (byte)str.ReadByte();

            headerVal[headerLen++] = x;

            if ((x & 0x80) == 0)
            { /* 0xxxxxxx */
                v = x;
                i = 0;
            }
            else if ((x & 0xC0) != 0 && (x & 0x20) == 0)
            { /* 110xxxxx */
                v = (ulong)(x & 0x1F);
                i = 1;
            }
            else if ((x & 0xE0) != 0 && (x & 0x10) == 0)
            { /* 1110xxxx */
                v = (ulong)x & 0x0F;
                i = 2;
            }
            else if ((x & 0xF0) != 0 && (x & 0x08) == 0)
            { /* 11110xxx */
                v = (ulong)x & 0x07;
                i = 3;
            }
            else if ((x & 0xF8) != 0 && (x & 0x04) == 0)
            { /* 111110xx */
                v = (ulong)x & 0x03;
                i = 4;
            }
            else if ((x & 0xFC) != 0 && (x & 0x02) == 0)
            { /* 1111110x */
                v = (ulong)x & 0x01;
                i = 5;
            }
            else if ((x & 0xFE) != 0 && (x & 0x01) == 0)
            { /* 11111110 */
                v = 0;
                i = 6;
            }
            else
            {
                //TODO: exception: number is not UTF-8 coded
                return 0;
            }

            for (; i > 0; i--)
            {
                x = (byte)str.ReadByte();

                headerVal[headerLen++] = x;

                if ((x & 0x80) == 0 || (x & 0x40) != 0)
                { /* 10xxxxxx */
                    //TODO: again: malformed UTF-8 number
                    return 0;
                }
                v <<= 6;
                v |= (ulong)(x & 0x3F);
            }
            return v;
        }

        public override string ToString()
        {

            return "Variable: " + variableLength + " size: " + blocksize + " SR: " + sampleRate + "Hz channels: " + channels + " CS: " + channelSetup + " sample: " + sampleSize + "b sequence: " + frameNumber;

        }
    }
}
