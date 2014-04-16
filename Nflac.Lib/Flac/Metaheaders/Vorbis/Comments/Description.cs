using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Metaheaders.Vorbis.Comments
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
