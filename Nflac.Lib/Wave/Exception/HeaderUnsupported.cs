using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Wave.Exception
{
    public class HeaderUnsupported : System.Exception
    {
        public HeaderUnsupported() : base()
        {

        }

        public HeaderUnsupported(string str)
            : base(str)
        {

        }
    }
}
