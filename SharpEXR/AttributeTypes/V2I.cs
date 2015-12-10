using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR.AttributeTypes {
    public struct V2I {
        public int V0;
        public int V1;

        public V2I(int v0, int v1) {
            V0 = v0;
            V1 = v1;
        }

        public override string ToString() {
            return string.Format("{0}: {1}, {2}", GetType().Name, V0, V1);
        }

        public int X { get { return V0; } }
        public int Y { get { return V1; } }
    }
}
