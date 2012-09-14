using IMAPI2.Interop;

namespace IMAPI2
{
    public class FormatWriteUpdateEventArgs
    {
        public FormatDataWriteAction _currentAction;
        public int _startLba;
        public int _sectorCount;
        public int _lastReadLba;
        public int _lastWrittenLba;
        public int _totalSystemBuffer;
        public int _usedSystemBuffer;
        public int _freeSystemBuffer;
        public int _elapsedTime;
        public int _remainingTime;
        public int _totalTime;

        // The starting logical block address for the current write operation.        
        public int StartLba
        {
            get { return _startLba; }
        }

        // The number of sectors being written for the current write operation.        
        public int SectorCount
        {
            get { return _sectorCount; }
        }

        // The last logical block address of data read for the current write operation.        
        public int LastReadLba
        {
            get { return _lastReadLba; }
        }

        // The last logical block address of data written for the current write operation        
        public int LastWrittenLba
        {
            get { return _lastWrittenLba; }
        }

        // The total bytes available in the system's cache buffer        
        public int TotalSystemBuffer
        {
            get { return _totalSystemBuffer; }
        }

        // The used bytes in the system's cache buffer        
        public int UsedSystemBuffer
        {
            get { return _usedSystemBuffer; }
        }

        // The free bytes in the system's cache buffer        
        public int FreeSystemBuffer
        {
            get { return _freeSystemBuffer; }
        }

        // The total elapsed time for the current write operation        
        public int ElapsedTime
        {
            get { return _elapsedTime; }
        }

        // The estimated time remaining for the write operation.        
        public int RemainingTime
        {
            get { return _remainingTime; }
        }

        // The estimated total time for the write operation.        
        public int TotalTime
        {
            get { return _totalTime; }
        }

        // The current write action.        
        FormatDataWriteAction CurrentAction
        {
            get { return _currentAction; }
        }

        internal FormatWriteUpdateEventArgs(IDiscFormat2DataEventArgs e)
        {
            _elapsedTime = e.ElapsedTime;
            _remainingTime = e.RemainingTime;
            _totalTime = e.TotalTime;
            _currentAction = (FormatDataWriteAction)e.CurrentAction;
            _startLba = e.StartLba;
            _sectorCount = e.SectorCount;
            _lastReadLba = e.LastReadLba;
            _lastWrittenLba = e.LastWrittenLba;
            _totalSystemBuffer = e.TotalSystemBuffer;
            _usedSystemBuffer = e.UsedSystemBuffer;
            _freeSystemBuffer = e.FreeSystemBuffer;
        }
    }
}
