using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR {
    public class EXRFile {
        public EXRVersion Version { get; protected set; }
        public List<EXRHeader> Headers { get; protected set; }
        public List<OffsetTable> OffsetTables { get; protected set; }
        public List<EXRPart> Parts { get; protected set; }

        public EXRFile() {
        }

        public void Read(IEXRReader reader) {
            // first four bytes of an OpenEXR file are always 0x76, 0x2f, 0x31 and 0x01 or 20000630
            var magicNumber = reader.ReadInt32();
            if (magicNumber != 20000630) {
                throw new EXRFormatException("Invalid or corrupt EXR layout: First four bytes were not 20000630.");
            }

            var versionValue = reader.ReadInt32();
            Version = new EXRVersion(versionValue);

            Headers = new List<EXRHeader>();
            if (Version.IsMultiPart) {
                while (true) {
                    var header = new EXRHeader();
                    header.Read(this, reader);
                    if (header.IsEmpty) {
                        break;
                    }
                    Headers.Add(header);
                }
                throw new NotImplementedException("Multi part EXR files are not currently supported");
            }
            else {
                if (Version.IsSinglePartTiled) {
                    throw new NotImplementedException("Tiled EXR files are not currently supported");
                }

                var header = new EXRHeader();
                header.Read(this, reader);
                Headers.Add(header);
            }

            OffsetTables = new List<OffsetTable>();
            foreach (var header in Headers) {
                int offsetTableSize;

                if (Version.IsMultiPart) {
                    offsetTableSize = header.ChunkCount;
                }
                else if (Version.IsSinglePartTiled) {
                    // TODO: Implement
                    offsetTableSize = 0;
                }
                else {
                    var compression = header.Compression;
                    var dataWindow = header.DataWindow;
                    var linesPerBlock = GetScanLinesPerBlock(compression);
                    var blockCount = (int)Math.Ceiling(dataWindow.Height / (double)linesPerBlock);

                    offsetTableSize = blockCount;
                }

                var table = new OffsetTable(offsetTableSize);
                table.Read(reader, offsetTableSize);
                OffsetTables.Add(table);
            }
        }

        public static int GetScanLinesPerBlock(EXRCompression compression) {
            switch (compression) {
                default:
                    return 1;
                case EXRCompression.ZIP:
                case EXRCompression.PXR24:
                    return 16;
                case EXRCompression.PIZ:
                case EXRCompression.B44:
                case EXRCompression.B44A:
                    return 32;
            }
        }

        public static int GetBytesPerPixel(ImageDestFormat format) {
            switch (format) {
                case ImageDestFormat.RGB16:
                case ImageDestFormat.BGR16:
                    return 6;
                case ImageDestFormat.RGB32:
                case ImageDestFormat.BGR32:
                    return 12;
                case ImageDestFormat.RGB8:
                case ImageDestFormat.BGR8:
                    return 3;
                case ImageDestFormat.PremultipliedRGBA16:
                case ImageDestFormat.PremultipliedBGRA16:
                case ImageDestFormat.RGBA16:
                case ImageDestFormat.BGRA16:
                    return 8;
                case ImageDestFormat.PremultipliedRGBA32:
                case ImageDestFormat.PremultipliedBGRA32:
                case ImageDestFormat.RGBA32:
                case ImageDestFormat.BGRA32:
                    return 16;
                case ImageDestFormat.PremultipliedRGBA8:
                case ImageDestFormat.PremultipliedBGRA8:
                case ImageDestFormat.RGBA8:
                case ImageDestFormat.BGRA8:
                    return 4;
            }
            throw new ArgumentException("Unrecognized destination format", "format");
        }

        public static int GetBitsPerPixel(ImageDestFormat format) {
            switch (format) {
                case ImageDestFormat.PremultipliedRGBA32:
                case ImageDestFormat.PremultipliedBGRA32:
                case ImageDestFormat.RGBA32:
                case ImageDestFormat.BGRA32:
                case ImageDestFormat.RGB32:
                case ImageDestFormat.BGR32:
                    return 32;
                case ImageDestFormat.PremultipliedRGBA8:
                case ImageDestFormat.PremultipliedBGRA8:
                case ImageDestFormat.RGBA8:
                case ImageDestFormat.BGRA8:
                case ImageDestFormat.RGB8:
                case ImageDestFormat.BGR8:
                    return 8;
                case ImageDestFormat.RGB16:
                case ImageDestFormat.BGR16:
                case ImageDestFormat.PremultipliedRGBA16:
                case ImageDestFormat.PremultipliedBGRA16:
                case ImageDestFormat.RGBA16:
                case ImageDestFormat.BGRA16:
                    return 16;
            }
            throw new ArgumentException("Unrecognized destination format", "format");
        }

#if DOTNET
        public static EXRFile FromFile(string file) {
            var reader = new EXRReader(new FileStream(file, FileMode.Open, FileAccess.Read));
            var result = FromReader(reader);
            reader.Dispose();
            return result;
        }
#endif

        public static EXRFile FromStream(Stream stream) {
            var reader = new EXRReader(new BinaryReader(stream));
            var result = FromReader(reader);
            reader.Dispose();
            return result;
        }

        public static EXRFile FromReader(IEXRReader reader) {
            var img = new EXRFile();
            img.Read(reader);

            img.Parts = new List<EXRPart>();
            for (int i = 0; i < img.Headers.Count; i++) {
                var part = new EXRPart(img.Version, img.Headers[i], img.OffsetTables[i]);
                img.Parts.Add(part);
                //part.ReadPixelData(reader);
            }

            return img;
        }
    }
}
