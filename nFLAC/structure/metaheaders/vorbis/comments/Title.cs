using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
{
    class Title : VorbisUserComment
    {
        public Title(string content) : base(VorbisCommentType.TITLE,content)
        {
        }

        public override string ToString()
        {
            return "Title: " + Comment;
        }
    }
}
