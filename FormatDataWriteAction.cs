using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMAPI2
{
    public enum FormatDataWriteAction
    {
        ValidatingMedia,
        FormattingMedia,
        InitializingHardware,
        CalibratingPower,
        WritingData,
        Finalization,
        Completed,
        Verifying
    }
}
