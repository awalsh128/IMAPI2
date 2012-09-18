using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMAPI2
{
    public class RecorderNotFoundException : RecorderNotSupportedException
    {
        public RecorderNotFoundException(string message)
            : base(message)
        {            
        }
    }
}
