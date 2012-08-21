using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
{
    class Location : VorbisUserComment
    {
        public Location(string content) : base(VorbisCommentType.LOCATION,content)
        {
        }

        public override string ToString()
        {
            return "Location: " + Comment;
        }
    }
}
