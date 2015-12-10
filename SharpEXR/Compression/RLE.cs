using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR.Compression {
    public static class RLE {
        public static int Uncompress(IEXRReader reader, int count, byte[] uncompressed) {
            var end = reader.Position + count;
            var maxLen = uncompressed.Length;
            var offset = 0;

            while (reader.Position < end) {
                int runcount = (sbyte)reader.ReadByte();
                if (runcount < 0) {
                    // raw
                    runcount = -runcount;
                    if (offset + runcount >= maxLen) {
                        reader.Position -= 1;
                        return 0;
                    }

                    reader.CopyBytes(uncompressed, offset, runcount);
                    offset += runcount;
                }
                else {
                    // run length
                    if (offset + runcount + 1 >= maxLen) {
                        reader.Position -= 1;
                        return 0;
                    }

                    var value = reader.ReadByte();
                    MemSet(uncompressed, offset, value, runcount + 1);
                    offset += runcount + 1;
                }
            }

            return offset;
        }

        public static void MemSet(byte[] array, int offset, byte value, int count) {
#if DOTNET
            if (array == null) {
                throw new ArgumentNullException("array");
            }

            int block = 32;
            int index = offset;
            int end = index + Math.Min(block, count);

            //Fill the initial array
            while (index < end) {
                array[index++] = value;
            }

            end = offset + count;
            while (index < end) {
                Buffer.BlockCopy(array, 0, array, index, Math.Min(block, end - index));
                index += block;
                block *= 2;
            }
#else
            var end = offset + count;
            for (int i = offset; i < end; i++) {
                array[i] = value;
            }
#endif
        }
    }
}
