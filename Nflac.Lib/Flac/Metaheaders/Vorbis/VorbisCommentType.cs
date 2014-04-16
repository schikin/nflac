using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Metaheaders.Vorbis
{
    enum VorbisCommentType : byte
    {
        TITLE, 
        VERSION,
        ALBUM,
        TRACKNUMBER,
        ARTIST,
        PERFORMER,
        COPYRIGHT,
        LICENSE,
        ORGANIZATION,
        DESCRIPTION,
        GENRE,
        DATE,
        LOCATION,
        CONTACT,
        ISRC,
        CUSTOM
    }
}
