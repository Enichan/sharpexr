using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR.AttributeTypes {
    public struct Box2I {
        public readonly int XMin;
        public readonly int YMin;
        public readonly int XMax;
        public readonly int YMax;

        public Box2I(int xMin, int yMin, int xMax, int yMax) {
            XMin = xMin;
            YMin = yMin;
            XMax = xMax;
            YMax = yMax;
        }

        public override string ToString() {
            return string.Format("{0}: ({1}, {2})-({3}, {4})", GetType().Name, XMin, YMin, XMax, YMax);
        }

        public int Width { get { return (XMax - XMin) + 1; } }
        public int Height { get { return (YMax - YMin) + 1; } }
    }
}
