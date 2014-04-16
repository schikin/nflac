using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Metaheaders.Vorbis.Comments
{
    class Contact : VorbisUserComment
    {
        public Contact(string content) : base(VorbisCommentType.CONTACT,content)
        {
        }

        public override string ToString()
        {
            return "Contact: " + Comment;
        }
    }
}
