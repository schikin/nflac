using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Metaheaders.Vorbis.Comments
{
    class Genre : VorbisUserComment
    {
        public Genre(string content) : base(VorbisCommentType.GENRE,content)
        {
        }

        public override string ToString()
        {
            return "Genre: " + Comment;
        }
    }
}
