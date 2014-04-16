using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Nflac.Audioformat
{
    /// <summary>
    /// Interface for any format (up to 32-bit signed samples)
    /// </summary>
    public interface IPCMStream
    {
        void ReadSample(int[] buffer);

        void Open();

        void Close();
    }
}
