using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
{
    class ISRC : VorbisUserComment
    {
        public ISRC(string content) : base(VorbisCommentType.ISRC,content)
        {
        }

        public override string ToString()
        {
            return "ISRC: " + Comment;
        }
    }
}
