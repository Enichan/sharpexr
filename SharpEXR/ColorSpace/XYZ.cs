using SharpEXR.AttributeTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// source http://www.ryanjuckett.com/programming/rgb-color-space-conversion/
namespace SharpEXR.ColorSpace {
    public struct tVec2 {
        public float X;
        public float Y;

        public tVec2(float x, float y) {
            X = x;
            Y = y;
        }
    }
    public struct tVec3 {
        public float X;
        public float Y;
        public float Z;

        public tVec3(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }
    }
    public struct tMat3x3 {
        public float
            M00, M01, M02,
            M10, M11, M12,
            M20, M21, M22;

        public void SetCol(int colIdx, tVec3 vec) {
            this[0, colIdx] = vec.X;
            this[1, colIdx] = vec.Y;
            this[2, colIdx] = vec.Z;
        }

        public bool Invert(out tMat3x3 result) {
            result = default(tMat3x3);

            // calculate the minors for the first row
            var minor00 = this[1, 1] * this[2, 2] - this[1, 2] * this[2, 1];
            var minor01 = this[1, 2] * this[2, 0] - this[1, 0] * this[2, 2];
            var minor02 = this[1, 0] * this[2, 1] - this[1, 1] * this[2, 0];

            // calculate the determinant
            var determinant = this[0, 0] * minor00 +
                              this[0, 1] * minor01 +
                              this[0, 2] * minor02;

            // check if the input is a singular matrix (non-invertable)
            // (note that the epsilon here was arbitrarily chosen)
            if (determinant > -0.000001f && determinant < 0.000001f) {
                return false;
            }

            // the inverse of inMat is (1 / determinant) * adjoint(inMat)
            var invDet = 1.0f / determinant;
            result[0, 0] = invDet * minor00;
            result[0, 1] = invDet * (this[2, 1] * this[0, 2] - this[2, 2] * this[0, 1]);
            result[0, 2] = invDet * (this[0, 1] * this[1, 2] - this[0, 2] * this[1, 1]);

            result[1, 0] = invDet * minor01;
            result[1, 1] = invDet * (this[2, 2] * this[0, 0] - this[2, 0] * this[0, 2]);
            result[1, 2] = invDet * (this[0, 2] * this[1, 0] - this[0, 0] * this[1, 2]);

            result[2, 0] = invDet * minor02;
            result[2, 1] = invDet * (this[2, 0] * this[0, 1] - this[2, 1] * this[0, 0]);
            result[2, 2] = invDet * (this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0]);

            return true;
        }

        public static tVec3 operator *(tMat3x3 mat, tVec3 vec) {
            return new tVec3(
                mat[0, 0] * vec.X + mat[0, 1] * vec.Y + mat[0, 2] * vec.Z,
                mat[1, 0] * vec.X + mat[1, 1] * vec.Y + mat[1, 2] * vec.Z,
                mat[2, 0] * vec.X + mat[2, 1] * vec.Y + mat[2, 2] * vec.Z
            );
        }

        public float this[int row, int col] {
            get {
                switch (row) {
                    case 0:
                        switch (col) {
                            default:
                                return M00;
                            case 1:
                                return M01;
                            case 2:
                                return M02;
                        }
                    case 1:
                        switch (col) {
                            default:
                                return M10;
                            case 1:
                                return M11;
                            case 2:
                                return M12;
                        }
                    default:
                        switch (col) {
                            default:
                                return M20;
                            case 1:
                                return M21;
                            case 2:
                                return M22;
                        }
                }
            }
            set {
                switch (row) {
                    case 0:
                        switch (col) {
                            default:
                                M00 = value;
                                break;
                            case 1:
                                M01 = value;
                                break;
                            case 2:
                                M02 = value;
                                break;
                        }
                        break;
                    case 1:
                        switch (col) {
                            default:
                                M10 = value;
                                break;
                            case 1:
                                M11 = value;
                                break;
                            case 2:
                                M12 = value;
                                break;
                        }
                        break;
                    default:
                        switch (col) {
                            default:
                                M20 = value;
                                break;
                            case 1:
                                M21 = value;
                                break;
                            case 2:
                                M22 = value;
                                break;
                        }
                        break;
                }
            }
        }
    }

    public static class XYZ {
        /// <summary>
        /// Convert a linear sRGB color to an sRGB color 
        /// </summary>
        public static tMat3x3 CalcColorSpaceConversion_RGB_to_XYZ(Chromaticities chromaticities) {
            return CalcColorSpaceConversion_RGB_to_XYZ(
                new tVec2(chromaticities.RedX, chromaticities.RedY),
                new tVec2(chromaticities.GreenX, chromaticities.GreenY),
                new tVec2(chromaticities.BlueX, chromaticities.BlueY),
                new tVec2(chromaticities.WhiteX, chromaticities.WhiteY));
        }

        /// <summary>
        /// Convert a linear sRGB color to an sRGB color 
        /// </summary>
        public static tMat3x3 CalcColorSpaceConversion_RGB_to_XYZ
        (
            tVec2 red_xy,           // xy chromaticity coordinates of the red primary
            tVec2 green_xy,         // xy chromaticity coordinates of the green primary
            tVec2 blue_xy,          // xy chromaticity coordinates of the blue primary
            tVec2 white_xy          // xy chromaticity coordinates of the white point
        )
        {
            tMat3x3 pOutput = new tMat3x3();

            // generate xyz chromaticity coordinates (x + y + z = 1) from xy coordinates
            tVec3 r = new tVec3(red_xy.X, red_xy.Y, 1.0f - (red_xy.X + red_xy.Y));
            tVec3 g = new tVec3(green_xy.X, green_xy.Y, 1.0f - (green_xy.X + green_xy.Y));
            tVec3 b = new tVec3(blue_xy.X, blue_xy.Y, 1.0f - (blue_xy.X + blue_xy.Y));
            tVec3 w = new tVec3(white_xy.X, white_xy.Y, 1.0f - (white_xy.X + white_xy.Y));
  
            // Convert white xyz coordinate to XYZ coordinate by letting that the white
            // point have and XYZ relative luminance of 1.0. Relative luminance is the Y
            // component of and XYZ color.
            //   XYZ = xyz * (Y / y) 
            w.X /= white_xy.Y;
            w.Y /= white_xy.Y;
            w.Z /= white_xy.Y;
  
            // Solve for the transformation matrix 'M' from RGB to XYZ
            // * We know that the columns of M are equal to the unknown XYZ values of r, g and b.
            // * We know that the XYZ values of r, g and b are each a scaled version of the known
            //   corresponding xyz chromaticity values.
            // * We know the XYZ value of white based on its xyz value and the assigned relative
            //   luminance of 1.0.
            // * We know the RGB value of white is (1,1,1).
            //                  
            //   white_XYZ = M * white_RGB
            //
            //       [r.x g.x b.x]
            //   N = [r.y g.y b.y]
            //       [r.z g.z b.z]
            //
            //       [sR 0  0 ]
            //   S = [0  sG 0 ]
            //       [0  0  sB]
            //
            //   M = N * S
            //   white_XYZ = N * S * white_RGB
            //   N^-1 * white_XYZ = S * white_RGB = (sR,sG,sB)
            //
            // We now have an equation for the components of the scale matrix 'S' and
            // can compute 'M' from 'N' and 'S'

            pOutput.SetCol(0, r);
            pOutput.SetCol(1, g);
            pOutput.SetCol(2, b);

            tMat3x3 invMat;
            pOutput.Invert(out invMat);

            tVec3 scale = invMat * w;
  
            pOutput[0, 0] *= scale.X;
            pOutput[1, 0] *= scale.X;
            pOutput[2, 0] *= scale.X;
  
            pOutput[0, 1] *= scale.Y;
            pOutput[1, 1] *= scale.Y;
            pOutput[2, 1] *= scale.Y;
  
            pOutput[0, 2] *= scale.Z;
            pOutput[1, 2] *= scale.Z;
            pOutput[2, 2] *= scale.Z;

            return pOutput;
        }
    }
}
