using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMAPI2
{
    public enum PhysicalMedia
    {
        Unknown = 0,
        CdRom = 1,
        CdR = 2,
        CdRw = 3,
        DvdRom = 4,
        DvdRam = 5,
        DvdPlusR = 6,
        DvdPlusRw = 7,
        DvdPlusRDualLayer = 8,
        DvdR = 9,
        DvdRw = 10,
        DvdRDualLayer = 11,
        Disk = 12,
        DvdPlusRwDualLayer = 13,
        HdDvdRom = 14,
        HdDvdR = 15,
        HdDvdRam = 0x10,
        BdRom = 0x11,
        BdR = 0x12,
        BdRE = 0x13,
        MAX = 0x13
    }
}
