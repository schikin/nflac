using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Metaheaders.Vorbis.Comments
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
