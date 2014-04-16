using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Exceptions
{
    class PCMStreamNotOpen : Exception
    {
        public PCMStreamNotOpen(String message) : base(message)
        {

        }

        public PCMStreamNotOpen(String message, Exception ex) : base(message,ex)
        {

        }

        public PCMStreamNotOpen() : base()
        {

        }
    }
    
}
