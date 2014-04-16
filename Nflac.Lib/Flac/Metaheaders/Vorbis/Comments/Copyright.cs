using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Metaheaders.Vorbis.Comments
{
    class Copyright : VorbisUserComment
    {
        public Copyright(string content) : base(VorbisCommentType.COPYRIGHT,content)
        {
        }

        public override string ToString()
        {
            return "Copyright: " + Comment;
        }
    }
}
