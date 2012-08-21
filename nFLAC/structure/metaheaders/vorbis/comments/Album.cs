using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
{
    class Album : VorbisUserComment
    {
        public Album(string content) : base(VorbisCommentType.ALBUM,content)
        {
        }

        public override string ToString()
        {
            return "Album: " + Comment;
        }
    }
}
