using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using org.nflac.structure.exceptions.stream;

namespace org.nflac.structure.util
{
    class BitReader
    {
        private Stream stream;

        private byte stored;

        private int numRead;

        private MemoryStream rememberStream = new MemoryStream();

        public MemoryStream RememberStream
        {
            get { return rememberStream; }
        }
        private bool isRecording;

        public bool IsRecording
        {
            get { return isRecording; }
            set 
            { 
            isRecording = value;

            if (isRecording)
            {
                for (; rememberStream.ReadByte() != -1; ) ;
            }

            }

        }

        public BitReader(Stream str)
        {
            stream = str;
        }

        public void ReadBit(ref long result)
        {
            if (numRead == 0)
            {
                int readRes = stream.ReadByte();

                if (readRes == -1)
                {
                    throw new EndOfStreamException();
                }

                stored = (byte) stream.ReadByte();

                if (isRecording)
                {
                    rememberStream.WriteByte(stored);
                }
            }


            numRead = (numRead + 1) % 8;

            result <<= 1;
            result |= (stored >> 7);
            stored <<= 1;
        }

        public void ReadBit(ref int result)
        {
            if (numRead == 0)
            {
                int readRes = stream.ReadByte();

                if (readRes == -1)
                {
                    throw new EndOfStreamException();
                }

                stored = (byte)readRes;

                if (isRecording)
                {
                    rememberStream.WriteByte(stored);
                }
            }

            numRead = (numRead + 1) % 8;

            result <<= 1;
            result |= (stored >> 7);
            stored <<= 1;
        }

        public void ReadBitLong(ref long result)
        {
            if (numRead == 0)
            {
                int readRes = stream.ReadByte();

                if (readRes == -1)
                {
                    throw new EndOfStreamException();
                }

                stored = (byte)readRes;

                if (isRecording)
                {
                    rememberStream.WriteByte(stored);
                }
            }

            numRead = (numRead + 1) % 8;

            result <<= 1;
            result |= (stored >> 7);
            stored <<= 1;
        }

        public int ReadBits(int num)
        {
            int result = 0;

            for (int i = 0; i < num; i++)
            {
                ReadBit(ref result);
            }

            return result;
        }

        public long ReadBitsLong(int num)
        {
            long result = 0;

            for (int i = 0; i < num; i++)
            {
                ReadBitLong(ref result);
            }

            return result;
        }

        public int ReadByte()
        {
            if (numRead == 0)
            {
                int ret = stream.ReadByte();

                if (ret == -1)
                {
                    throw new EndOfStreamException();
                }

                byte b = (byte) ret;

                if (isRecording)
                {
                    rememberStream.WriteByte(b);
                }

                return b;
            }
            else
            {
                return ReadBits(8);
            }
        }

        public uint ReadUnary()
        {
            uint res = 0;

            try
            {

                while (ReadBits(1) == 0)
                {
                    res++;
                }

            }
            catch (EndOfStreamException ex)
            {
                //TODO: correct handling
                return 0;
            }

            return res;
        }

        public ulong ReadUnaryLong()
        {
            ulong res = 0;

            try
            {

                while (ReadBits(1) == 0)
                {
                    res++;
                }

            }
            catch (EndOfStreamException ex)
            {
                throw new UnexpectedEndOfStreamException("Unexpected end of stream: unary number never finished");
            }

            return res;
        }

        public int ReadRice(int param)
        {
            //TODO: fixme

            uint ures = ReadRiceUnsigned(param);

            if ((ures & 0x1)!=0)
            {
                return -((int)(ures >> 1)) - 1;
            }
            else
            {
                return (int)(ures >> 1);
            }
            

        }

        public uint ReadRiceUnsigned(int param)
        {
            uint q = ReadUnary();
            uint r = 0;

            r = (uint)ReadBits(param);

            return (q << param | r);
        }

        public ushort ReadUInt16()
        {
            ushort ret = 0;
            ushort buff = 0;

            for (int i = 0; i < 2; i++)
            {
                ret = (ushort)(ret << 8);
                buff = (ushort)ReadByte();
                ret |= buff;
            }

            return ret;
            
        }

        public uint ReadUInt32()
        {
            uint ret = 0;
            uint buff = 0;

            for (int i = 0; i < 4; i++)
            {
                ret = (uint)(ret << 8);
                buff = (uint)ReadByte();
                ret |= buff;
            }

            return ret;

        }

        public ulong ReadUInt64()
        {
            ulong ret = 0;
            ulong buff = 0;

            for (int i = 0; i < 8; i++)
            {
                ret = (ulong)(ret << 8);
                buff = (ulong)ReadByte();
                ret |= buff;
            }

            return ret;

        }

        public short ReadInt16()
        {
            short ret = 0;
            short buff = 0;

            for (int i = 0; i < 2; i++)
            {
                ret = (short)(ret << 8);
                buff = (short)ReadByte();
                ret |= buff;
            }

            return ret;

        }

        public int ReadInt32()
        {
            int ret = 0;
            int buff = 0;

            for (int i = 0; i < 4; i++)
            {
                ret = (int)(ret << 8);
                buff = (int)ReadByte();
                ret |= buff;
            }

            return ret;

        }

        public long ReadInt64()
        {
            long ret = 0;
            long buff = 0;

            for (int i = 0; i < 8; i++)
            {
                ret = (long)(ret << 8);
                buff = (long)ReadByte();
                ret |= buff;
            }

            return ret;

        }
    }
}
