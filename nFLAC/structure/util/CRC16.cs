using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace org.nflac.structure.util
{
    class CRC16
    {
        private byte[] seekTable = new byte[256];

        private static CRC16 instance;

        private const int polynomial = 0x18005;

        public ushort Checksum(params byte[] val)
        {
            ushort c = 0;

            foreach (byte b in val)
            {
                c = seekTable[c ^ b];
            }

            return c;
        }

        public ushort Checksum(Stream str)
        {
            ushort c = 0;
            int b;

            while ((b = str.ReadByte()) != -1)
            {
                c = seekTable[c ^ b];
            }

            return c;
        }

        private CRC16()
        {
  
            for (int i = 0; i < 256; ++i)
            {
                int curr = i;

                for (int j = 0; j < 8; ++j)
                {
                    if ((curr & 0x80) != 0)
                    {
                        curr = (curr << 1) ^ (int)polynomial;
                    }
                    else
                    {
                        curr <<= 1;
                    }
                }

                seekTable[i] = (byte)curr;
            }
            
        }

        public static CRC16 Instance
        {
            get {
                if (instance == null)
                {
                    instance = new CRC16();
                }
                return CRC16.instance; }
        }
    }
}
