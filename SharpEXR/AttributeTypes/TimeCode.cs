using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR.AttributeTypes {
    public struct TimeCode {
        public readonly uint TimeAndFlags;
        public readonly uint UserData;

        public TimeCode(uint timeAndFlags, uint userData) {
            TimeAndFlags = timeAndFlags;
            UserData = userData;
        }
    }
}
