using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
{
    class Organization : VorbisUserComment
    {
        public Organization(string content) : base(VorbisCommentType.ORGANIZATION,content)
        {
        }

        public override string ToString()
        {
            return "Organization: " + Comment;
        }
    }
}
