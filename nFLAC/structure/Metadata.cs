using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using org.nflac.structure.metaheaders;

namespace org.nflac.structure
{
    public abstract class Metadata
    {
        private BlockType type;
        private int length;

        private bool isLastBlock;

        public bool IsLastBlock
        {
            get { return isLastBlock; }
        }
        
        protected abstract void Parse(byte[] payload);

        public static Metadata Decode(Stream stream)
        {
            byte header;

            header = (byte) stream.ReadByte();

            bool lastBlock;

            lastBlock = (header & 0x80) == 0x80;
 
            if (lastBlock)
            {
                header ^= 0x80;
            }

            BlockType type = (BlockType) header;

            int buff;
            int len = 0;

            for (int i = 0; i < 3; i++)
            {
                len = len << 8;
                buff = stream.ReadByte();
                len |= buff;
            }

            byte[] payload = new byte[len];

            for (int i = 0; i < len; i++)
            {
                payload[i] = (byte) stream.ReadByte();
            }

            Metadata instance;

            switch (type) {
                case BlockType.STREAMINFO:
                    instance = new StreamInfo();
                    break;
                case BlockType.APPLICATION:
                    instance = new Application();
                    break;
                case BlockType.CUESHEET:
                    instance = new CueSheet();
                    break;
                case BlockType.PADDING:
                    instance = new Padding();
                    break;
                case BlockType.PICTURE:
                    instance = new Picture();
                    break;
                case BlockType.SEEKTABLE:
                    instance = new SeekTable();
                    break;
                case BlockType.UNKNOWN:
                    instance = new Unknown();
                    break;
                case BlockType.VORBIS_COMMENT:
                    instance = new VorbisComment();
                    break;
                default:
                    instance = new Reserved();
                    break;
            }

            instance.isLastBlock = lastBlock;
            instance.length = len;
            instance.type = type;
            instance.Parse(payload);

            return instance;
        }

        public override string ToString()
        {
            
            return "Header type: " + type + " length: " + length + " last block flag: " + isLastBlock;

        }


    }
}
