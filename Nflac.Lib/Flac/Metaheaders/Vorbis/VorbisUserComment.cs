using Org.Nflac.Flac.Metaheaders.Vorbis.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Metaheaders.Vorbis
{
    class VorbisUserComment
    {
        private VorbisCommentType type;
        private string comment;
        private string originalTitle;
        
        public VorbisUserComment(string type, string content){
            this.type = TypeFromString(type);
            this.originalTitle = type;
            this.comment = content;
        }

        internal VorbisUserComment(VorbisCommentType type, string content)
        {
            this.type = type;
            this.comment = content;
        }

        internal static VorbisCommentType TypeFromString(string vorbisTitle)
        {
            VorbisCommentType ret;
            try
            {
                ret = (VorbisCommentType)Enum.Parse(typeof(VorbisCommentType), vorbisTitle, true);
            }
            catch (Exception ex)
            {
                ret = VorbisCommentType.CUSTOM;
            }

            return ret;
        }

        public static VorbisUserComment Parse(byte[] payload)
        {
            string str = Encoding.UTF8.GetString(payload);

            string[] commSplit = str.Split('=');

            VorbisCommentType type = TypeFromString(commSplit[0]);

            switch (type)
            {
                case VorbisCommentType.ALBUM:
                    return new Album(commSplit[1]);
                case VorbisCommentType.ARTIST:
                    return new Artist(commSplit[1]);
                case VorbisCommentType.CONTACT:
                    return new Contact(commSplit[1]);
                case VorbisCommentType.COPYRIGHT:
                    return new Copyright(commSplit[1]);
                case VorbisCommentType.DATE:
                    return new Date(commSplit[1]);
                case VorbisCommentType.DESCRIPTION:
                    return new Description(commSplit[1]);
                case VorbisCommentType.GENRE:
                    return new Genre(commSplit[1]);
                case VorbisCommentType.ISRC:
                    return new ISRC(commSplit[1]);
                case VorbisCommentType.LICENSE:
                    return new License(commSplit[1]);
                case VorbisCommentType.LOCATION:
                    return new Location(commSplit[1]);
                case VorbisCommentType.ORGANIZATION:
                    return new Organization(commSplit[1]);
                case VorbisCommentType.PERFORMER:
                    return new Performer(commSplit[1]);
                case VorbisCommentType.TITLE:
                    return new Title(commSplit[1]);
                case VorbisCommentType.TRACKNUMBER:
                    return new TrackNumber(commSplit[1]);
                case VorbisCommentType.VERSION:
                    return new Org.Nflac.Flac.Metaheaders.Vorbis.Comments.Version(commSplit[1]);    
                default:
                    Custom ret = new Custom(commSplit[1]);
                    ret.originalTitle = commSplit[0];
                    return ret;
            }
        }

        public VorbisCommentType Type
        {
            get { return type; }
        }

        public string Comment
        {
            get { return comment; }
        }

        public string OriginalTitle
        {
            get { return originalTitle; }
        }

    }
}
