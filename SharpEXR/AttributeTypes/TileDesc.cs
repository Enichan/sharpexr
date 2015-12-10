using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR.AttributeTypes {
    public struct TileDesc {
        public readonly uint XSize;
        public readonly uint YSize;
        public readonly LevelMode LevelMode;
        public readonly RoundingMode RoundingMode;

        public TileDesc(uint xSize, uint ySize, byte mode) {
            XSize = xSize;
            YSize = ySize;

            // mode is levelMode + roundingMode * 16
            var roundingMode = (mode & 0xF0) >> 4;
            var levelMode = mode & 0xF;

            RoundingMode = (SharpEXR.RoundingMode)roundingMode;
            LevelMode = (SharpEXR.LevelMode)levelMode;
        }

        public override string ToString() {
            return string.Format("{0}: XSize={1}, YSize={2}", GetType().Name, XSize, YSize);
        }
    }
}
