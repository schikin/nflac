using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Metaheaders.Vorbis.Comments
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
