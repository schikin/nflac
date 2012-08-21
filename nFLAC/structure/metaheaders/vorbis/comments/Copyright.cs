using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
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
