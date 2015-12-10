using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR.AttributeTypes {
    public struct V2F {
        public float V0;
        public float V1;

        public V2F(float v0, float v1) {
            V0 = v0;
            V1 = v1;
        }

        public override string ToString() {
            return string.Format("{0}: {1}, {2}", GetType().Name, V0, V1);
        }

        public float X { get { return V0; } }
        public float Y { get { return V1; } }
    }
}
