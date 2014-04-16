﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Org.Nflac.Flac.Exceptions
{
    /// <summary>
    /// Base exception class for all format-related exceptions
    /// </summary>
    public class IncorrectStreamException : Exception
    {
        public IncorrectStreamException(String message) : base(message)
        {

        }

        public IncorrectStreamException(String message, Exception ex) : base(message,ex)
        {

        }

        public IncorrectStreamException()
            : base("Stream reading error")
        {

        }

        public IncorrectStreamException(Stream stream)
            : base("Stream reading error at position " + stream.Position)
        {
        }
    }
}
