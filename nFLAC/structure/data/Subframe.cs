using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using org.nflac.structure.util;
using org.nflac.structure.metaheaders;

namespace org.nflac.structure.data
{
    class Subframe
    {
        internal const int MAX_BLOCKSIZE = 65535;

        private SubframeType type;

        internal SubframeType Type
        {
            get { return type; }
            set { type = value; }
        }

        private uint order;

        private uint wastedBits;

        private int[] samples = new int[MAX_BLOCKSIZE];
        private int[] residuals = new int[MAX_BLOCKSIZE];
        private int numResiduals;
        private int numSamples;

        private int channelNum;

        public uint WastedBits
        {
            get { return wastedBits; }
            set { wastedBits = value; }
        }

        public void Decode(Frame fr, StreamInfo streamInfo, BitReader str, int bitsPerSample, int channelNum)
        {
            byte header;

            this.channelNum = channelNum;

            header = (byte) str.ReadByte();

            if ((header & 0x80) != 0)
            {
                //TODO: handle corrupt bit
                return;
            }

            byte subtype = (byte)((header & 0x7E) >> 1);

            if (subtype == 0)
            {
                type = SubframeType.Constant;
            }
            else if (subtype == 1)
            {
                type = SubframeType.Verbatim;
            }
            else if ((subtype >= 8)&&(subtype<=12))
            {
                type = SubframeType.Fixed;
                order = (uint)(subtype & 0x7);
            }
            else if ((subtype & 0x20) != 0)
            {
                type = SubframeType.LPC;
                order = (uint)(subtype & 0x1F) + 1;
            }
            else
            {
                //TODO: handle reserved frame types
                return;
            }

            wastedBits = 0;

            if ((header & 0x1) != 0)
            {
                int buff = 0;

                do
                {
                    str.ReadBit(ref buff);
                    wastedBits++;
                } while (buff == 0);
            }

            numSamples = 0;
            numResiduals = 0;

            switch (type)
            {
                case SubframeType.Constant:
                    ProcessConstant(fr, str, bitsPerSample);
                    break;
                case SubframeType.Verbatim:
                    ProcessVerbatim(fr, str, bitsPerSample);
                    break;
                case SubframeType.Fixed:
                    ProcessFixed(fr, str, bitsPerSample);
                    break;
                case SubframeType.LPC:
                    ProcessLPC(fr, str, bitsPerSample);
                    break;

            }
        }

        private void ProcessConstant(Frame fr, BitReader str, int bitsPerSample)
        {
            int value = 0;

            value = TruncateTC(str.ReadBits(bitsPerSample),bitsPerSample);

            for (ulong i = 0; i < fr.Blocksize; i++)
            {
                fr.output[channelNum, (int)i] = value;
            }
        }

        private void ProcessVerbatim(Frame fr, BitReader str, int bitsPerSample)
        {
            for (ulong i = 0; i < fr.Blocksize; i++)
            {
                fr.output[channelNum, (int)i] = TruncateTC(str.ReadBits(bitsPerSample),bitsPerSample);
            }
        }

        private void ProcessFixed(Frame fr, BitReader str, int bitsPerSample)
        {
            for (int i = 0; i < order; i++)
            {
                samples[numSamples] = TruncateTC(str.ReadBits(bitsPerSample),bitsPerSample);
                fr.output[channelNum, (int)numSamples] = samples[numSamples];
                numSamples++;
            }

            ReadRice(fr, str);

            //TODO: restore signal, bunch of checks
            for (int i = 0; i < numResiduals; i++)
            {
                switch (order)
                {
                    case 0:
                        //x[i] = residual[i]
                        fr.output[channelNum, (int)numSamples++] = residuals[i];
                        break;
                    case 1:
                        //x[i] = residual[i] + x[i-1]
                        fr.output[channelNum, (int)numSamples] = (residuals[i] + fr.output[channelNum, (int)numSamples - 1]);
                        numSamples++;
                        break;
                    case 2:
                        //x[i] = residual[i] + 2x[i-1] - x[i-2]
                        fr.output[channelNum, (int)numSamples] = (residuals[i] + 2 * fr.output[channelNum, (int)numSamples - 1] - fr.output[channelNum, (int)numSamples - 2]);
                        numSamples++;
                        break;
                    case 3:
                        //x[i] = residual[i] + 3x[i-1] - 3x[i-2] + x[i-3]
                        fr.output[channelNum, (int)numSamples] = (residuals[i] + 3 * fr.output[channelNum, (int)numSamples - 1] - 3 * fr.output[channelNum, (int)numSamples - 2] + fr.output[channelNum, (int)numSamples - 3]);
                        numSamples++;
                        break;
                    case 4:
                        //x[i] = residual[i] + 4x[i-1] - 6x[i-2] + 4x[i-3] - x[i-4]
                        fr.output[channelNum, (int)numSamples] = (residuals[i] + 4 * fr.output[channelNum, (int)numSamples - 1] - 6 * fr.output[channelNum, (int)numSamples - 2] + 4 * fr.output[channelNum, (int)numSamples - 3] - fr.output[channelNum, (int)numSamples - 4]);
                        numSamples++;
                        break;
                }
            }
        }

        private void ProcessLPC(Frame fr, BitReader str, int bitsPerSample)
        {
           
            for (int i = 0; i < order; i++)
            {
                samples[numSamples] = TruncateTC(str.ReadBits(bitsPerSample),bitsPerSample);
                fr.output[channelNum, (int)numSamples] = samples[numSamples];
                numSamples++;
            }

            int precision = str.ReadBits(4) + 1;
            int shift = TruncateTC(str.ReadBits(5),5);

            int[] coefficients = new int[32]; //there cannot be more than 32 LPC coefficients

            for (int i = 0; i < order; i++)
            {
                coefficients[i] = TruncateTC(str.ReadBits(precision),precision);
            }

            ReadRice(fr, str);

            //TODO: restore signal, bunch of checks
            for (int i = 0; i < numResiduals; i++)
            {
                long signal = 0;

                for (int j = 0; j < order; j++)
                {
                    signal += (long)coefficients[j] * (long)fr.output[channelNum, numSamples - j - 1];
                }

                fr.output[channelNum, numSamples++] = (int)((long)residuals[i] + (signal >> shift));

                if (numSamples == 264)
                {
                    //breakpoint
                }
            }

            
        }

        private int TruncateTC(int num, int len)
        {   
            num <<= (32 - len);
            num >>= (32 - len);

            return num;
        }

        private void ReadRice(Frame fr, BitReader str)
        {
            int riceType = str.ReadBits(2);
            int riceParam = 0;
            int riceParamLen = 0;
            int partOrder = 0;
            int partSize = 0;

            switch (riceType)
            {
                case 0:
                    riceParamLen = 4;
                    break;
                case 1:
                    riceParamLen = 5;
                    break;
                default:
                    //TODO: exception, handle reserved rice types
                    return;
            }

            partOrder = str.ReadBits(4);
            int numPartitions = 1 << partOrder;
            int fullMask = (1 << (riceParamLen + 1)) - 1;

            for (int i = 0; i < numPartitions; i++)
            {
                riceParam = str.ReadBits(riceParamLen);

                if (partOrder == 0)
                {
                    partSize = (int) ((int)fr.Blocksize - order);
                }
                else if (i == 0)
                {
                    partSize = (int)(Math.Floor(fr.Blocksize / (double)(1 << partOrder))) - (int)(order);
                }
                else
                {
                    partSize = (int)(Math.Floor(fr.Blocksize / (double)(1 << partOrder)));
                }

                if (riceParam == fullMask)
                {
                    //unencoded
                    int residualLen = str.ReadBits(5);

                    for (int j = 0; j < partSize; j++)
                    {
                        residuals[numResiduals++] = str.ReadBits(residualLen);
                    }
                }
                else
                {
                    uint uRiceParam = (uint)riceParam;

                    for (int j = 0; j < partSize; j++)
                    {
                        residuals[numResiduals++] = str.ReadRice(riceParam);
                    }
                }
            }

        }

        public override string ToString()
        {
            return "Subframe. Type: " + type + " samples: "+numSamples + " residuals: "+numResiduals;
        }
    }
}
