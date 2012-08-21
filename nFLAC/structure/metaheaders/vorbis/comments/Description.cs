using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
{
    class Description : VorbisUserComment
    {
        public Description(string content) : base(VorbisCommentType.DESCRIPTION,content)
        {
        }

        public override string ToString()
        {
            return "Description: " + Comment;
        }
    }
}
