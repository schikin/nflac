using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace org.nflac.structure.util
{
    class BitWriter
    {
        private Stream stream;

        private int numWritten;

        private byte stored;

        public BitWriter(Stream str)
        {
            stream = str;
        }

        public void WriteBit(bool bit)
        {
            stored <<= 1;
            if(bit)
            {
                stored |= 1;
            }

            numWritten = (numWritten + 1) % 8;

            if (numWritten == 0)
            {
                stream.WriteByte(stored);
                stored = 0;
            }
        }

        public void WriteBit(byte bit)
        {
            if (bit == 1)
            {
                WriteBit(true);
            }
            else
            {
                WriteBit(false);
            }
        }

        public void WriteBit(int bit)
        {
            if (bit == 1)
            {
                WriteBit(true);
            }
            else
            {
                WriteBit(false);
            }
        }

        public void WriteBit(long bit)
        {
            if (bit == 1)
            {
                WriteBit(true);
            }
            else
            {
                WriteBit(false);
            }
        }

        public void WriteBits(byte bits, int num)
        {
            for (int i = 0; i < num; i++)
            {
                WriteBit((bits >> num-1-i) & 0x1);
            }
        }

        public void WriteBits(short bits, int num)
        {
            for (int i = 0; i < num; i++)
            {
                WriteBit((bits >> num - 1 - i) & 0x1);
            }
        }

        public void WriteBits(int bits, int num)
        {
            for (int i = 0; i < num; i++)
            {
                WriteBit((bits >> num - 1 - i) & 0x1);
            }
        }

        public void WriteBits(long bits, int num)
        {
            for (int i = 0; i < num; i++)
            {
                WriteBit((bits >> num - 1 - i) & 0x1);
            }
        }

        public void WriteBits(sbyte bits, int num)
        {
            for (int i = 0; i < num; i++)
            {
                WriteBit((bits >> num - 1 - i) & 0x1);
            }
        }

        public void WriteBits(ushort bits, int num)
        {
            for (int i = 0; i < num; i++)
            {
                WriteBit((bits >> num - 1 - i) & 0x1);
            }
        }

        public void WriteBits(uint bits, int num)
        {
            for (int i = 0; i < num; i++)
            {
                WriteBit((bits >> num - 1 - i) & 0x1);
            }
        }

        public void WriteBits(ulong bits, int num)
        {
            for (int i = 0; i < num; i++)
            {
                WriteBit((long)(bits >> num - 1 - i) & 0x1);
            }
        }

        public void WriteLE(ushort value)
        {
            for (int i = 0; i < 2; i++)
            {
                WriteBits(value & 0xff, 8);
                value >>= 8;
            }
        }

        public void WriteLE(uint value)
        {
            for (int i = 0; i < 4; i++)
            {
                WriteBits(value & 0xff, 8);
                value >>= 8;
            }
        }

        public void WriteLE(ulong value)
        {
            for (int i = 0; i < 8; i++)
            {
                WriteBits(value & 0xff, 8);
                value >>= 8;
            }
        }
    }
}
