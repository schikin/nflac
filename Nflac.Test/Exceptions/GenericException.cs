﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Tests.Exceptions
{
    class GenericException : Exception
    {
        public GenericException(String message)
            : base(message)
        {

        }

        public GenericException(String message, Exception ex)
            : base(message, ex)
        {

        }

        public GenericException()
            : base("General-type exception")
        {

        }
    }
}
