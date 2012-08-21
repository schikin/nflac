using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.nflac.structure.metaheaders.vorbis;

namespace org.nflac.structure.metaheaders
{
    class VorbisComment : Metadata
    {
        private string vendor;

        private List<VorbisUserComment> comments = new List<VorbisUserComment>();

        public string Vendor
        {
            get { return vendor; }
        }

        public List<VorbisUserComment> Comments
        {
            get { return comments; }            
        }

        protected override void Parse(byte[] payload)
        {
            uint vendorLength = 0;

            for (int i = 0; i < 4; i++)
            {
                //vendorLength = vendorLength << 8;
                vendorLength |= (uint) (payload[i] << i*8); //pffff... little-endian
            }

            ulong trueLen = vendorLength; //nah, let's treat octet = byte

            byte[] vendorCode = new byte[trueLen];

            for (ulong i = 0; i < trueLen; i++)
            {
                vendorCode[i] = payload[i + 4];
            }

            vendor = System.Text.Encoding.UTF8.GetString(vendorCode);

            uint numComments = 0;
            int cnt = 0;

            for (ulong i = 4+trueLen; i < trueLen+8; i++)
            {
                
                //numComments = numComments << 8;
                numComments |= (uint) (payload[i] << cnt*8);
                cnt++;
            }


            ulong commOffset = 0;

            for (uint i = 0; i < numComments; i++)
            {
                uint comLen = 0;
                int cntInner = 0;

                for (ulong j = 0; j < 4; j++)
                {
                    //comLen = comLen << 8;
                    comLen |= (uint) (payload[trueLen+8+j+commOffset] << cntInner*8);
                    cntInner++;
                }


                ulong comByteLen = comLen;
                byte[] comByte = new byte[comByteLen];

                for (ulong j = 0; j < comByteLen; j++)
                {
                    comByte[j] = payload[trueLen + 12 + commOffset + j];
                }

                comments.Add(VorbisUserComment.Parse(comByte));

                commOffset += (comLen + 4);

            }
        }

        public override string ToString()
        {
            string ret = base.ToString();

            ret += "\n Total of " + comments.Count + " comments: ";

            foreach (VorbisUserComment cmt in comments)
            {
                ret += "\n " + cmt;
            }

            return ret;
        }
    }
}
