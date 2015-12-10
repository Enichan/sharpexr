using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR {
    public enum EXRCompression {
        None = 0,
        RLE = 1,
        ZIPS = 2,
        ZIP = 3,
        PIZ = 4,
        PXR24 = 5,
        B44 = 6,
        B44A = 7
    }
}
