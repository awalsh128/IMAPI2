using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMAPI2
{
    public class FormatEraseUpdateEventArgs
    {
        private int _elapsedSeconds;
        private int _totalEstimatedSeconds;

        public int ElapsedSeconds 
        {
            get { return _elapsedSeconds; }
        }
        public int TotalEstimatedSeconds 
        {
            get { return _totalEstimatedSeconds; }
        }

        public FormatEraseUpdateEventArgs(int elapsedSeconds, int totalEstimatedSeconds)
        {
            _elapsedSeconds = elapsedSeconds;
            _totalEstimatedSeconds = totalEstimatedSeconds;
        }
    }
}
