using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace org.nflac.tests.exceptions
{
    class XMLAssertException : AssertFailedException
    {
        public XMLAssertException(String message) : base(message)
        {

        }

        public XMLAssertException(String message, Exception ex) : base(message,ex)
        {

        }

        public XMLAssertException()
            : base("Error parsing XML file")
        {

        }
        
    }
}
