using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.metaheaders.vorbis.comments
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
