using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
{
    class Custom : VorbisUserComment
    {
        public Custom(string content) : base(VorbisCommentType.CUSTOM,content)
        {
        }

        public override string ToString()
        {
            return "Custom comment: " + OriginalTitle + "=" + Comment;
        }
    }
}
