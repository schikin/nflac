using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
{
    class License : VorbisUserComment
    {
        public License(string content) : base(VorbisCommentType.LICENSE,content)
        {
        }

        public override string ToString()
        {
            return "License: " + Comment;
        }
    }
}
