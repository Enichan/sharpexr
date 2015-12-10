using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR.ColorSpace {
    public static class Gamma {
        /// <summary>
        /// Convert a gamma corrected color channel to a linear color channel.
        /// </summary>
        public static float Expand(float nonlinear) {
            return (float)Math.Pow(nonlinear, 2.2);
        }

        /// <summary>
        /// Convert a linear color channel to a gamma corrected color channel.
        /// </summary>
        public static float Compress(float linear) {
            return (float)Math.Pow(linear, 1.0 / 2.2);
        }

        /// <summary>
        /// Convert a gamma corrected color to a linear color.
        /// </summary>
        public static void Expand(ref tVec3 pColor) {
            pColor.X = Expand(pColor.X);
            pColor.Y = Expand(pColor.Y);
            pColor.Z = Expand(pColor.Z);
        }

        /// <summary>
        /// Convert a linear color to a gamma corrected color.
        /// </summary>
        public static void Compress(ref tVec3 pColor) {
            pColor.X = Compress(pColor.X);
            pColor.Y = Compress(pColor.Y);
            pColor.Z = Compress(pColor.Z);
        }

        /// <summary>
        /// Convert a gamma corrected color to a linear color.
        /// </summary>
        public static void Expand(ref float r, ref float g, ref float b) {
            r = Expand(r);
            g = Expand(g);
            b = Expand(b);
        }

        /// <summary>
        /// Convert a linear color to a gamma corrected color.
        /// </summary>
        public static void Compress(ref float r, ref float g, ref float b) {
            r = Compress(r);
            g = Compress(g);
            b = Compress(b);
        }

        /// <summary>
        /// Convert a gamma corrected color to a linear color.
        /// </summary>
        public static tVec3 Expand(float r, float g, float b) {
            var vec = new tVec3(r, g, b);
            Expand(ref vec);
            return vec;
        }

        /// <summary>
        /// Convert a linear color to a gamma corrected color.
        /// </summary>
        public static tVec3 Compress(float r, float g, float b) {
            var vec = new tVec3(r, g, b);
            Compress(ref vec);
            return vec;
        }

        /// <summary>
        /// Convert an sRGB color channel to a linear sRGB color channel.
        /// </summary>
        public static float Expand_sRGB(float nonlinear) {
            return (nonlinear <= 0.04045f)
                   ? (nonlinear / 12.92f)
                   : ((float)Math.Pow((nonlinear + 0.055f) / 1.055f, 2.4f));
        }

        /// <summary>
        /// Convert a linear sRGB color channel to a sRGB color channel.
        /// </summary>
        public static float Compress_sRGB(float linear) {
            return (linear <= 0.0031308f)
                   ? (12.92f * linear)
                   : (1.055f * (float)Math.Pow(linear, 1.0f / 2.4f) - 0.055f);
        }

        /// <summary>
        /// Convert an sRGB color to a linear sRGB color.
        /// </summary>
        public static void Expand_sRGB(ref tVec3 pColor) {
            pColor.X = Expand_sRGB(pColor.X);
            pColor.Y = Expand_sRGB(pColor.Y);
            pColor.Z = Expand_sRGB(pColor.Z);
        }

        /// <summary>
        /// Convert a linear sRGB color to an sRGB color.
        /// </summary>
        public static void Compress_sRGB(ref tVec3 pColor) {
            pColor.X = Compress_sRGB(pColor.X);
            pColor.Y = Compress_sRGB(pColor.Y);
            pColor.Z = Compress_sRGB(pColor.Z);
        }

        /// <summary>
        /// Convert an sRGB color to a linear sRGB color.
        /// </summary>
        public static void Expand_sRGB(ref float r, ref float g, ref float b) {
            r = Expand_sRGB(r);
            g = Expand_sRGB(g);
            b = Expand_sRGB(b);
        }

        /// <summary>
        /// Convert a linear sRGB color to an sRGB color.
        /// </summary>
        public static void Compress_sRGB(ref float r, ref float g, ref float b) {
            r = Compress_sRGB(r);
            g = Compress_sRGB(g);
            b = Compress_sRGB(b);
        }

        /// <summary>
        /// Convert an sRGB color to a linear sRGB color.
        /// </summary>
        public static tVec3 Expand_sRGB(float r, float g, float b) {
            var vec = new tVec3(r, g, b);
            Expand_sRGB(ref vec);
            return vec;
        }

        /// <summary>
        /// Convert a linear sRGB color to an sRGB color.
        /// </summary>
        public static tVec3 Compress_sRGB(float r, float g, float b) {
            var vec = new tVec3(r, g, b);
            Compress_sRGB(ref vec);
            return vec;
        }
    }
}
