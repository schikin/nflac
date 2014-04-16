using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Exceptions
{
    /// <summary>
    /// Malformed metadata
    /// </summary>
    class MalformedFileException : IncorrectStreamException
    {
        public MalformedFileException(String message) : base(message)
        {

        }

        public MalformedFileException(String message, Exception ex) : base(message,ex)
        {

        }

        public MalformedFileException() : base()
        {

        }
    }
}
