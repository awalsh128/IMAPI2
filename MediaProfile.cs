namespace IMAPI2
{
    public enum MediaProfile
    {
        Invalid = 0,
        NonRemovableDisk = 1,
        RemovableDisk = 2,
        MoErasable = 3,
        MoWriteOnce = 4,
        AsMo = 5,
        CdRom = 8,
        CdR = 9,
        CdRewritable = 10,
        DvdRom = 0x10,
        DvdR = 0x11,
        DvdRam = 0x12,
        DvdRewritable = 0x13,
        DvdRwSequential = 0x14,
        DvdRDualSequential = 0x15,
        DvdRDualLAYERJUMP = 0x16,
        DvdPlusRw = 0x1a,
        DvdPlusR = 0x1b,
        DdCdRom = 0x20,
        DdCdR = 0x21,
        DdCdRewritable = 0x22,
        DvdPlusRwDual = 0x2a,
        DvdPlusRDual = 0x2b,
        BdRom = 0x40,
        BdRSequential = 0x41,
        BdRRandomRecording = 0x42,
        BdRewritable = 0x43,
        HdDvdRom = 0x50,
        HdDvdR = 0x51,
        HdDvdRam = 0x52,
        NonStandard = 0xffff                  
    }
}
