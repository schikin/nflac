using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Wave.Exception
{
    public class CompressionUnsupported : System.Exception
    {
        public CompressionUnsupported()
            : base()
        {

        }

        public CompressionUnsupported(string msg)
            : base(msg)
        {

        }
    }
}
