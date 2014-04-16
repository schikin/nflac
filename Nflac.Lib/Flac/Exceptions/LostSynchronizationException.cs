using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Exceptions
{
    /// <summary>
    /// Failed to synch at designated position
    /// </summary>
    class LostSynchronizationException: IncorrectStreamException
    {
        public LostSynchronizationException(String message) : base(message)
        {

        }

        public LostSynchronizationException(String message, Exception ex) : base(message,ex)
        {

        }

        public LostSynchronizationException()
            : base()
        {

        }
    }
}
