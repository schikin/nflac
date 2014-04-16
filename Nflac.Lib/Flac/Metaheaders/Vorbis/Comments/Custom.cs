using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Metaheaders.Vorbis.Comments
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
