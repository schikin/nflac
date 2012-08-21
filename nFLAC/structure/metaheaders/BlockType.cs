using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders
{
    enum BlockType : byte
    {
        STREAMINFO = 0, PADDING = 1, APPLICATION = 2, SEEKTABLE = 3,
        VORBIS_COMMENT = 4, CUESHEET = 5, PICTURE = 6, UNKNOWN = 127
    };
}
