using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
{
    class Artist : VorbisUserComment
    {
        public Artist(string content) : base(VorbisCommentType.ARTIST,content)
        {
        }

        public override string ToString()
        {
            return "Artist: " + Comment;
        }
    }
}
