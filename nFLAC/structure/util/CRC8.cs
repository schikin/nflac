using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.util
{
    class CRC8
    {
        private byte[] seekTable = new byte[256];

        private static CRC8 instance;

        private const int polynomial = 0x107;

        public byte Checksum(params byte[] val)
        {
            byte c = 0;

            foreach (byte b in val)
            {
                c = seekTable[c ^ b];
            }

            return c;
        }

        public Boolean Check(byte crc, params byte[] val)
        {
            byte[] param = new byte[val.Length + 1];

            for (int i = 0; i < val.Length; i++)
            {
                param[i] = val[i];
            }

            param[val.Length] = crc;

            return Checksum(param)==0;
        }

        private CRC8()
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

        public static CRC8 Instance
        {
            get {
                if (instance == null)
                {
                    instance = new CRC8();
                }
                return CRC8.instance; }
        }
    }
}
