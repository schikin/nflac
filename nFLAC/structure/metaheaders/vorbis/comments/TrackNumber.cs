using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
{
    class TrackNumber : VorbisUserComment
    {
        public TrackNumber(string content) : base(VorbisCommentType.TRACKNUMBER,content)
        {
        }

        public override string ToString()
        {
            return "TrackNumber: " + Comment;
        }
    }
}
