using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
{
    class Performer : VorbisUserComment
    {
        public Performer(string content) : base(VorbisCommentType.PERFORMER,content)
        {
        }

        public override string ToString()
        {
            return "Performer: " + Comment;
        }
    }
}
