using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Metaheaders.Vorbis.Comments
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
