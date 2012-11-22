using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.nflac.structure.exceptions;
using org.nflac.structure.exceptions.stream;

namespace org.nflac.structure.metaheaders
{
    class SeekTable : Metadata
    {
        private List<SeekPoint> points = new List<SeekPoint>();

        internal List<SeekPoint> Points
        {
            get { return points; }
        }

        protected override void Parse(byte[] payload)
        {
            

            if (payload.Length % 18 != 0)
            {
                //length should be divisible by 18
                throw new MalformedFileException("Seektable meta header malformed: size should be divisible by 18");
            }

            for (int i = 0; i < (payload.Length / 18); i++)
            {
                ulong number = 0;
                ulong offset = 0;
                ushort size = 0;

                for (int j = 0; j < 8; j++)
                {
                    number = number << 8;
                    number |= payload[i * 18 + j];
                }

                for (int j = 8; j < 16; j++)
                {
                    offset = offset << 8;
                    offset |= payload[i * 18 + j];
                }

                for (int j = 16; j < 18; j++)
                {
                    size = (ushort) (size << 8);
                    size |= payload[i * 18 + j];
                }

                points.Add(new SeekPoint(number, offset, size));
            }
        }

        public override string ToString()
        {
            string ret;
            ret = base.ToString();

            ret += "\n Total of " + points.Count + "points:";

            foreach (SeekPoint sp in points)
            {
                ret += "\n " + sp.ToString();
            }

            return ret;
        }
    }
}
