using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMAPI2
{
    public class ImageUpdateEventArgs : EventArgs
    {
        private string _currentFile;
        private int _copiedSectors; 
        private int _totalSectors;

        public string CurrentFile
        {
            get { return _currentFile; }
        }
        public int CopiedSectors 
        {
            get { return _copiedSectors; }
        }
        public int TotalSectors
        {
            get { return _totalSectors; }
        }

        public ImageUpdateEventArgs(string currentFile, int copiedSectors, int totalSectors)
        {
            _currentFile = currentFile;
            _copiedSectors = copiedSectors;
            _totalSectors = totalSectors;    
        }
    }
}
