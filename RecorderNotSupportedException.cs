using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMAPI2
{
    class RecorderNotSupportedException : NotSupportedException
    {
        public RecorderNotSupportedException(string message)
            : base(message)
        {            
        }
    }
}
