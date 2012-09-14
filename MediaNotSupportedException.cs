using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMAPI2
{
    public class MediaNotSupportedException : NotSupportedException
    {
        public MediaNotSupportedException(string message) : base(message)
        {            
        }
    }
}
