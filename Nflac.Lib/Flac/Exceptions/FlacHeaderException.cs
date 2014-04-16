using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Exceptions
{
    /// <summary>
    /// FLAC header not found in place
    /// </summary>
    public class FlacHeaderException : IncorrectStreamException
    {
        public FlacHeaderException(string str)
            : base(str)
        {

        }

        public FlacHeaderException()
        {

        }
    }
}
