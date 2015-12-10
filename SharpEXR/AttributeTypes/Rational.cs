using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR.AttributeTypes {
    public struct Rational {
        public readonly int Numerator;
        public readonly uint Denominator;

        public Rational(int numerator, uint denominator) {
            Numerator = numerator;
            Denominator = denominator;
        }

        public override string ToString() {
            return string.Format("{0}/{1}", Numerator, Denominator);
        }

        public double Value { get { return (double)Numerator / Denominator; } }
    }
}
