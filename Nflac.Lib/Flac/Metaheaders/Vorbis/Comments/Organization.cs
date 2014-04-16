using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Metaheaders.Vorbis.Comments
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
