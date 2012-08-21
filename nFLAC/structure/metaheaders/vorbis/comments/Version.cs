using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
{
    class Version : VorbisUserComment
    {
        public Version(string content) : base(VorbisCommentType.VERSION,content)
        {
        }

        public override string ToString()
        {
            return "Version: " + Comment;
        }
    }
}
