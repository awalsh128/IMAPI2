using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMAPI2
{
    public interface IMediaNode
    {
        /// <summary>
        /// Gets the display name of the file or directory.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the full path of the file or directory.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the size of the file or directory to the next largest sector.
        /// </summary>
        long SizeOnDisc { get; }

        /// <summary>
        /// Gets the Icon of the file or directory.
        /// </summary>
        System.Drawing.Image FileIcon { get; }
    }
}
