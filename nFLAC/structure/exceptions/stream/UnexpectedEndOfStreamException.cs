using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.exceptions.stream
{
    /// <summary>
    /// Unexpected EOF
    /// </summary>
    class UnexpectedEndOfStreamException: IncorrectStreamException
    {
        public UnexpectedEndOfStreamException(String message) : base(message)
        {

        }

        public UnexpectedEndOfStreamException(String message, Exception ex) : base(message,ex)
        {

        }

        public UnexpectedEndOfStreamException()
            : base()
        {

        }
    }
}
