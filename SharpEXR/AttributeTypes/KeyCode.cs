using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR.AttributeTypes {
    public struct KeyCode {
        public readonly int FilmMfcCode;
        public readonly int FilmType;
        public readonly int Prefix;
        public readonly int Count;
        public readonly int PerfOffset;
        public readonly int PerfsPerFrame;
        public readonly int PerfsPerCount;

        public KeyCode(int filmMfcCode, int filmType, int prefix, int count, int perfOffset, int perfsPerFrame, int perfsPerCount) {
            FilmMfcCode = filmMfcCode;
            FilmType = filmType;
            Prefix = prefix;
            Count = count;
            PerfOffset = perfOffset;
            PerfsPerFrame = perfsPerFrame;
            PerfsPerCount = perfsPerCount;
        }
    }
}
