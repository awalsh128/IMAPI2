using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMAPI2
{
    public enum MediaState
    {        
        Unknown = 0x0000,
        OverwriteOnly = 0x0001,
        RandomlyWritable = 0x0001,
        Blank = 0x0002,
        Appendable = 0x0004,
        FinalSession = 0x0008,
        InformationalMask = 0x000e,
        Damaged = 0x0400,
        EraseRequired = 0x0800,
        NonEmptySession = 0x1000,
        WriteProtected = 0x2000,
        Finalized = 0x4000,
        UnsupportedMedia = 0x8000,
        UnsupportedMask = 0xfc00
    }
}
