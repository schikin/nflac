using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace org.nflac.structure.util
{
    class StreamReader
    {
        public static ushort ReadUShort(Stream stream)
        {
            ushort ret = 0;
            ushort buff = 0;

            for (int i = 0; i < 2; i++)
            {
                ret = (ushort) (ret << 8);
                buff = (ushort) stream.ReadByte();
                ret |= buff;
            }

            return ret;
        }

        public static uint ReadUInt(Stream stream)
        {
            uint ret = 0;
            uint buff = 0;

            for (int i = 0; i < 4; i++)
            {
                ret = (ushort)(ret << 8);
                buff = (ushort)stream.ReadByte();
                ret |= buff;
            }

            return ret;
        }

        public static ulong ReadULong(Stream stream)
        {
            ulong ret = 0;
            ulong buff = 0;

            for (int i = 0; i < 8; i++)
            {
                ret = (ulong)(ret << 8);
                buff = (ulong)stream.ReadByte();
                ret |= buff;
            }

            return ret;
        }
       
    }
}
