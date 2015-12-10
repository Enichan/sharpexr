using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR.AttributeTypes {
    public struct Chromaticities {
        public readonly float RedX;
        public readonly float RedY;
        public readonly float GreenX;
        public readonly float GreenY;
        public readonly float BlueX;
        public readonly float BlueY;
        public readonly float WhiteX;
        public readonly float WhiteY;

        public Chromaticities(float redX, float redY, float greenX, float greenY, float blueX, float blueY, float whiteX, float whiteY) {
            RedX = redX;
            RedY = redY;
            GreenX = greenX;
            GreenY = greenY;
            BlueX = blueX;
            BlueY = blueY;
            WhiteX = whiteX;
            WhiteY = whiteY;
        }
    }
}
