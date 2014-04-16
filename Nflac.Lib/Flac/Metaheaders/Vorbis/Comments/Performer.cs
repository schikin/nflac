using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Metaheaders.Vorbis.Comments
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
