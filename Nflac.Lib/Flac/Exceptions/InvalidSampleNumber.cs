using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Flac.Exceptions
{
    public class InvalidSampleNumber : System.Exception
    {
        public InvalidSampleNumber(string str)
            : base(str)
        {

        }

        public InvalidSampleNumber()
        {

        }
    }
}
