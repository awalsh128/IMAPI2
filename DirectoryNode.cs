using Microsoft.Win32.Interop;
using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IMAPI2
{
    public class DirectoryNode : IMediaNode
    {
        private Image _fileIcon;
        private string _name;
        private List<IMediaNode> _nodes;
        private string _path;
        private long _sizeOnDisc;

        public string Name
        {
            get { return _name; }
        }

        public List<IMediaNode> Nodes
        {
            get { return _nodes; }
        }

        public string Path
        {
            get { return _path; }
        }

        public long SizeOnDisc
        {
            get { return _sizeOnDisc; }
        }

        public Image FileIcon
        {
            get { return _fileIcon; }
        }

        public DirectoryNode(string path)
        {
            if (!Directory.Exists(path))            
                throw new FileNotFoundException("The directory added to MediaDirectory was not found.", path);            

            _path = path;
            _nodes = new List<IMediaNode>();            
            _name = new FileInfo(_path).Name;

            string[] files = Directory.GetFiles(_path);
            foreach (string file in files)            
                _nodes.Add(new FileNode(file));            

            string[] directories = Directory.GetDirectories(_path);
            foreach (string directory in directories)            
                _nodes.Add(new DirectoryNode(directory));            

            _sizeOnDisc = 0;
            foreach (IMediaNode node in _nodes)            
                _sizeOnDisc += node.SizeOnDisc;            

            //
            // Get the Directory icon
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
