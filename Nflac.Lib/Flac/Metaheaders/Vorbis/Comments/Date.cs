﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Metaheaders.Vorbis.Comments
{
    class Date : VorbisUserComment
    {
        public Date(string content) : base(VorbisCommentType.DATE,content)
        {
        }

        public override string ToString()
        {
            return "Date: " + Comment;
        }
    }
}
