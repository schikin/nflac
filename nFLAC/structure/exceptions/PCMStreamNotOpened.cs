using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.exceptions
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
