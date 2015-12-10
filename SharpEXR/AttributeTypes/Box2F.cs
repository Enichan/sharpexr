using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR.AttributeTypes {
    public struct Box2F {
        public readonly float XMin;
        public readonly float YMin;
        public readonly float XMax;
        public readonly float YMax;

        public Box2F(float xMin, float yMin, float xMax, float yMax) {
            XMin = xMin;
            YMin = yMin;
            XMax = xMax;
            YMax = yMax;
        }

        public override string ToString() {
            return string.Format("{0}: ({1}, {2})-({3}, {4})", GetType().Name, XMin, YMin, XMax, YMax);
        }

        public float Width { get { return (XMax - XMin) + 1; } }
        public float Height { get { return (YMax - YMin) + 1; } }
    }
}
