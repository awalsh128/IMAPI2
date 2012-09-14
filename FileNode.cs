using Microsoft.Win32.Interop;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace IMAPI2
{
    public class FileNode : IMediaNode
    {
        private Image _fileIcon;
        private string _name;
        private string _path;
        private long _sizeOnDisc;

        public Image FileIcon
        {
            get { return _fileIcon; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Path
        {
            get { return _path; }
        }

        public long SizeOnDisc
        {
            get { return _sizeOnDisc; }
        }

        public FileNode(string path)
        {
            if (!File.Exists(path))            
                throw new FileNotFoundException("The file added to MediaFile was not found.", path);                        

            _fileIcon = null;
            _path = path;

            FileInfo fileInfo = new FileInfo(_path);
            _name = fileInfo.Name;
            _sizeOnDisc = fileInfo.Length;

            //
            // Get the File icon
            //
            Shell.SHFILEINFO shinfo = new Shell.SHFILEINFO();
            IntPtr hImg = Shell.SHGetFileInfo(_path, 0, ref shinfo,
                (uint)Marshal.SizeOf(shinfo), Shell.SHGFI_ICON | Shell.SHGFI_SMALLICON);

            if (shinfo.hIcon != null)
            {
                //The icon is returned in the hIcon member of the shinfo struct
                System.Drawing.IconConverter imageConverter = new System.Drawing.IconConverter();
                System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
                try
                {
                    _fileIcon = (System.Drawing.Image)imageConverter.ConvertTo(icon, typeof(System.Drawing.Image));
                }
                catch (NotSupportedException)
                {                    
                }

                Shell.DestroyIcon(shinfo.hIcon);
            }
        }

    }
}
