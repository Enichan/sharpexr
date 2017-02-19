using SharpEXR.AttributeTypes;
using SharpEXR.ColorSpace;
using SharpEXR.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR {
    public delegate IEXRReader ParallelReaderCreationDelegate();

    public class EXRPart {
        public readonly EXRVersion Version;
        public readonly EXRHeader Header;
        public readonly OffsetTable Offsets;
        public readonly PartType Type;

        public readonly Box2I DataWindow;
        private bool hasData;

        private Dictionary<string, float[]> floatChannels;
        public Dictionary<string, float[]> FloatChannels { 
            get {
                return floatChannels;
            }
            protected set {
                floatChannels = value;
            }
        }
        private Dictionary<string, Half[]> halfChannels;
        public Dictionary<string, Half[]> HalfChannels {
            get {
                return halfChannels;
            }
            protected set {
                halfChannels = value;
            }
        }

        public EXRPart(EXRVersion version, EXRHeader header, OffsetTable offsets) {
            Version = version;
            Header = header;
            Offsets = offsets;

            if (Version.IsMultiPart) {
                Type = header.Type;
            }
            else {
                Type = version.IsSinglePartTiled ? PartType.Tiled : PartType.ScanLine;
            }

            DataWindow = Header.DataWindow;
            FloatChannels = new Dictionary<string, float[]>();
            HalfChannels = new Dictionary<string, Half[]>();

            foreach (var channel in header.Channels) {
                if (channel.Type == PixelType.Float) {
                    FloatChannels[channel.Name] = new float[DataWindow.Width * DataWindow.Height];
                }
                else if (channel.Type == PixelType.Half) {
                    HalfChannels[channel.Name] = new Half[DataWindow.Width * DataWindow.Height];
                }
                else {
                    throw new NotImplementedException("Only 16 and 32 bit floating point EXR images are supported.");
                }
            }
        }

        protected virtual void CheckHasData() {
            if (!hasData) {
                throw new InvalidOperationException("Call EXRPart.Open before performing image operations.");
            }
        }

        /// <summary>
        /// Gets RGB color channels as an interleaved array of Halfs. If this EXRPart's HasAlpha property is true,
        /// this will include the alpha channel in the array.
        /// </summary>
        public Half[] GetHalfs(ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma) {
            return GetHalfs(channels, premultiplied, gamma, HasAlpha);
        }

        /// <summary>
        /// Gets RGB color channels as an interleaved array of Halfs. If includeAlpha is true,
        /// this will include the alpha channel in the array.
        /// </summary>
        public Half[] GetHalfs(ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma, bool includeAlpha) {
            ImageSourceFormat srcFormat;
            if (HalfChannels.ContainsKey("R") && HalfChannels.ContainsKey("G") && HalfChannels.ContainsKey("B")) {
                srcFormat = includeAlpha ? ImageSourceFormat.HalfRGBA : ImageSourceFormat.HalfRGB;
            }
            else if (FloatChannels.ContainsKey("R") && FloatChannels.ContainsKey("G") && FloatChannels.ContainsKey("B")) {
                srcFormat = includeAlpha ? ImageSourceFormat.SingleRGBA : ImageSourceFormat.SingleRGB;
            }
            else {
                throw new EXRFormatException("Unrecognized EXR image format, did not contain half/single RGB color channels");
            }
            return GetHalfs(srcFormat, channels, premultiplied, gamma);
        }

        public Half[] GetHalfs(ImageSourceFormat srcFormat, ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma) {
            ImageDestFormat destFormat;
            if (srcFormat == ImageSourceFormat.HalfRGBA || srcFormat == ImageSourceFormat.SingleRGBA) {
                if (premultiplied) {
                    destFormat =
                        channels == ChannelConfiguration.BGR ?
                        ImageDestFormat.PremultipliedBGRA16 :
                        ImageDestFormat.PremultipliedRGBA16;
                }
                else {
                    destFormat =
                        channels == ChannelConfiguration.BGR ?
                        ImageDestFormat.BGRA16 :
                        ImageDestFormat.RGBA16;
                }
            }
            else {
                destFormat =
                    channels == ChannelConfiguration.BGR ?
                    ImageDestFormat.BGR16 :
                    ImageDestFormat.RGB16;
            }

            var bytesPerPixel = EXRFile.GetBytesPerPixel(destFormat);
            var channelCount =
                srcFormat == ImageSourceFormat.SingleRGB || srcFormat == ImageSourceFormat.HalfRGB ? 3 : 4;

            var bytes = GetBytes(srcFormat, destFormat, gamma, DataWindow.Width * bytesPerPixel);
            Half[] halfs = new Half[bytes.Length / 2];
            Buffer.BlockCopy(bytes, 0, halfs, 0, bytes.Length);
            return halfs;
        }

        /// <summary>
        /// Gets RGB color channels as an interleaved array of floats. If this EXRPart's HasAlpha property is true,
        /// this will include the alpha channel in the array.
        /// </summary>
        public float[] GetFloats(ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma) {
            return GetFloats(channels, premultiplied, gamma, HasAlpha);
        }

        /// <summary>
        /// Gets RGB color channels as an interleaved array of floats. If includeAlpha is true,
        /// this will include the alpha channel in the array.
        /// </summary>
        public float[] GetFloats(ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma, bool includeAlpha) {
            ImageSourceFormat srcFormat;
            if (HalfChannels.ContainsKey("R") && HalfChannels.ContainsKey("G") && HalfChannels.ContainsKey("B")) {
                srcFormat = includeAlpha ? ImageSourceFormat.HalfRGBA : ImageSourceFormat.HalfRGB;
            }
            else if (FloatChannels.ContainsKey("R") && FloatChannels.ContainsKey("G") && FloatChannels.ContainsKey("B")) {
                srcFormat = includeAlpha ? ImageSourceFormat.SingleRGBA : ImageSourceFormat.SingleRGB;
            }
            else {
                throw new EXRFormatException("Unrecognized EXR image format, did not contain half/single RGB color channels");
            }
            return GetFloats(srcFormat, channels, premultiplied, gamma);
        }

        public float[] GetFloats(ImageSourceFormat srcFormat, ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma) {
            ImageDestFormat destFormat;
            if (srcFormat == ImageSourceFormat.HalfRGBA || srcFormat == ImageSourceFormat.SingleRGBA) {
                if (premultiplied) {
                    destFormat = 
                        channels == ChannelConfiguration.BGR ?
                        ImageDestFormat.PremultipliedBGRA32 :
                        ImageDestFormat.PremultipliedRGBA32;
                }
                else {
                    destFormat = 
                        channels == ChannelConfiguration.BGR ?
                        ImageDestFormat.BGRA32 :
                        ImageDestFormat.RGBA32;
                }
            }
            else {
                destFormat =
                    channels == ChannelConfiguration.BGR ?
                    ImageDestFormat.BGR32 :
                    ImageDestFormat.RGB32;
            }

            var bytesPerPixel = EXRFile.GetBytesPerPixel(destFormat);
            var channelCount = 
                srcFormat == ImageSourceFormat.SingleRGB || srcFormat == ImageSourceFormat.HalfRGB ? 3 : 4;

            var bytes = GetBytes(srcFormat, destFormat, gamma, DataWindow.Width * bytesPerPixel);
            float[] floats = new float[bytes.Length / sizeof(float)];
            Buffer.BlockCopy(bytes, 0, floats, 0, bytes.Length);
            return floats;
        }

        public byte[] GetBytes(ImageDestFormat destFormat, GammaEncoding gamma) {
            return GetBytes(destFormat, gamma, DataWindow.Width * EXRFile.GetBytesPerPixel(destFormat));
        }

        public byte[] GetBytes(ImageDestFormat destFormat, GammaEncoding gamma, int stride) {
            ImageSourceFormat srcFormat;
            if (HalfChannels.ContainsKey("R") && HalfChannels.ContainsKey("G") && HalfChannels.ContainsKey("B")) {
                srcFormat = HalfChannels.ContainsKey("A") ? ImageSourceFormat.HalfRGBA : ImageSourceFormat.HalfRGB;
            }
            else if (FloatChannels.ContainsKey("R") && FloatChannels.ContainsKey("G") && FloatChannels.ContainsKey("B")) {
                srcFormat = FloatChannels.ContainsKey("A") ? ImageSourceFormat.SingleRGBA : ImageSourceFormat.SingleRGB;
            }
            else {
                throw new EXRFormatException("Unrecognized EXR image format, did not contain half/single RGB color channels");
            }
            return GetBytes(srcFormat, destFormat, gamma, stride);
        }

        public byte[] GetBytes(ImageSourceFormat srcFormat, ImageDestFormat destFormat, GammaEncoding gamma) {
            return GetBytes(srcFormat, destFormat, gamma, DataWindow.Width * EXRFile.GetBytesPerPixel(destFormat));
        }

        public byte[] GetBytes(ImageSourceFormat srcFormat, ImageDestFormat destFormat, GammaEncoding gamma, int stride) {
            CheckHasData();

            int bytesPerPixel = EXRFile.GetBytesPerPixel(destFormat);
            int bitsPerPixel = EXRFile.GetBitsPerPixel(destFormat);

            if (stride < bytesPerPixel * DataWindow.Width) {
                throw new ArgumentException("Stride was lower than minimum", "stride");
            }
            byte[] buffer = new byte[stride * DataWindow.Height];

            var padding = stride - bytesPerPixel * DataWindow.Width;

            bool isHalf = srcFormat == ImageSourceFormat.HalfRGB || srcFormat == ImageSourceFormat.HalfRGBA;
            bool sourceAlpha = false;
            bool destinationAlpha =
                destFormat == ImageDestFormat.BGRA16 ||
                destFormat == ImageDestFormat.BGRA32 ||
                destFormat == ImageDestFormat.BGRA8 ||
                destFormat == ImageDestFormat.PremultipliedBGRA16 ||
                destFormat == ImageDestFormat.PremultipliedBGRA32 ||
                destFormat == ImageDestFormat.PremultipliedBGRA8 ||
                destFormat == ImageDestFormat.PremultipliedRGBA16 ||
                destFormat == ImageDestFormat.PremultipliedRGBA32 ||
                destFormat == ImageDestFormat.PremultipliedRGBA8 ||
                destFormat == ImageDestFormat.RGBA16 ||
                destFormat == ImageDestFormat.RGBA32 ||
                destFormat == ImageDestFormat.RGBA8;
            bool premultiplied =
                destFormat == ImageDestFormat.PremultipliedBGRA16 ||
                destFormat == ImageDestFormat.PremultipliedBGRA32 ||
                destFormat == ImageDestFormat.PremultipliedBGRA8 ||
                destFormat == ImageDestFormat.PremultipliedRGBA16 ||
                destFormat == ImageDestFormat.PremultipliedRGBA32 ||
                destFormat == ImageDestFormat.PremultipliedRGBA8;
            bool bgra =
                destFormat == ImageDestFormat.BGR16 ||
                destFormat == ImageDestFormat.BGR32 ||
                destFormat == ImageDestFormat.BGR8 ||
                destFormat == ImageDestFormat.BGRA16 ||
                destFormat == ImageDestFormat.BGRA32 ||
                destFormat == ImageDestFormat.BGRA8 ||
                destFormat == ImageDestFormat.PremultipliedBGRA16 ||
                destFormat == ImageDestFormat.PremultipliedBGRA32 ||
                destFormat == ImageDestFormat.PremultipliedBGRA8;

            Half[] hr, hg, hb, ha;
            float[] fr, fg, fb, fa;
            hr = hg = hb = ha = null;
            fr = fg = fb = fa = null;

            if (isHalf) {
                if (!HalfChannels.ContainsKey("R")) {
                    throw new ArgumentException("Half type channel R not found", "srcFormat");
                }
                if (!HalfChannels.ContainsKey("G")) {
                    throw new ArgumentException("Half type channel G not found", "srcFormat");
                }
                if (!HalfChannels.ContainsKey("B")) {
                    throw new ArgumentException("Half type channel B not found", "srcFormat");
                }
                hr = HalfChannels["R"];
                hg = HalfChannels["G"];
                hb = HalfChannels["B"];

                if (srcFormat == ImageSourceFormat.HalfRGBA) {
                    if (!HalfChannels.ContainsKey("A")) {
                        throw new ArgumentException("Half type channel A not found", "srcFormat");
                    }
                    ha = HalfChannels["A"];
                    sourceAlpha = true;
                }
            }
            else {
                if (!FloatChannels.ContainsKey("R")) {
                    throw new ArgumentException("Single type channel R not found", "srcFormat");
                }
                if (!FloatChannels.ContainsKey("G")) {
                    throw new ArgumentException("Single type channel G not found", "srcFormat");
                }
                if (!FloatChannels.ContainsKey("B")) {
                    throw new ArgumentException("Single type channel B not found", "srcFormat");
                }
                fr = FloatChannels["R"];
                fg = FloatChannels["G"];
                fb = FloatChannels["B"];

                if (srcFormat == ImageSourceFormat.HalfRGBA) {
                    if (!FloatChannels.ContainsKey("A")) {
                        throw new ArgumentException("Single type channel A not found", "srcFormat");
                    }
                    fa = FloatChannels["A"];
                    sourceAlpha = true;
                }
            }

#if !PARALLEL
            int srcIndex = 0;
            int destIndex = 0;

            BinaryWriter writer = new BinaryWriter(new MemoryStream(buffer));

            for (int y = 0; y < DataWindow.Height; y++, destIndex += padding) {
                GetScanlineBytes(bytesPerPixel, destIndex, srcIndex, isHalf, destinationAlpha, sourceAlpha,
                    hr, hg, hb, ha, fr, fg, fb, fa,
                    bitsPerPixel, gamma, premultiplied, bgra, buffer, writer);
                destIndex += DataWindow.Width * bytesPerPixel;
                srcIndex += DataWindow.Width;
            }

            writer.Dispose();
            writer.BaseStream.Dispose();
#else
            var actions = (from y in Enumerable.Range(0, DataWindow.Height) select (Action)(() => {
                var destIndex = stride * y;
                var srcIndex = DataWindow.Width * y;

                using (var stream = new MemoryStream(buffer)) {
                    using (var writer = new BinaryWriter(stream)) {
                        GetScanlineBytes(bytesPerPixel, destIndex, srcIndex, isHalf, destinationAlpha, sourceAlpha,
                            hr, hg, hb, ha, fr, fg, fb, fa,
                            bitsPerPixel, gamma, premultiplied, bgra, buffer, writer);
                    }
                }
            })).ToArray();
            Parallel.Invoke(actions);
#endif

            return buffer;
        }

        private void GetScanlineBytes(
            int bytesPerPixel, int destIndex, int srcIndex, bool isHalf, bool destinationAlpha, bool sourceAlpha,
            Half[] hr, Half[] hg, Half[] hb, Half[] ha, float[] fr, float[] fg, float[] fb, float[] fa,
            int bitsPerPixel, GammaEncoding gamma, bool premultiplied, bool bgra, byte[] buffer, BinaryWriter writer
        ) {
            writer.Seek(destIndex, SeekOrigin.Begin);
            for (int x = 0; x < DataWindow.Width; x++, destIndex += bytesPerPixel, srcIndex++) {
                float r, g, b, a;

                // get source channels as floats
                if (isHalf) {
                    r = hr[srcIndex];
                    g = hg[srcIndex];
                    b = hb[srcIndex];

                    if (destinationAlpha) {
                        a = sourceAlpha ? (float)ha[srcIndex] : 1.0f;
                    }
                    else {
                        a = 1.0f;
                    }
                }
                else {
                    r = fr[srcIndex];
                    g = fg[srcIndex];
                    b = fb[srcIndex];

                    if (destinationAlpha) {
                        a = sourceAlpha ? fa[srcIndex] : 1.0f;
                    }
                    else {
                        a = 1.0f;
                    }
                }

                // convert to destination format
                if (bitsPerPixel == 8) {
                    byte r8, g8, b8, a8 = 255;

                    if (gamma == GammaEncoding.Linear) {
                        if (premultiplied) {
                            r8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(r * a * 255 + 0.5)));
                            g8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(g * a * 255 + 0.5)));
                            b8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(b * a * 255 + 0.5)));
                            a8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(a * 255 + 0.5)));
                        }
                        else {
                            r8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(r * 255 + 0.5)));
                            g8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(g * 255 + 0.5)));
                            b8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(b * 255 + 0.5)));
                            if (destinationAlpha) {
                                a8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(a * 255 + 0.5)));
                            }
                        }
                    }
                    else if (gamma == GammaEncoding.Gamma) {
                        if (premultiplied) {
                            r8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(Gamma.Compress(r) * a * 255 + 0.5)));
                            g8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(Gamma.Compress(g) * a * 255 + 0.5)));
                            b8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(Gamma.Compress(b) * a * 255 + 0.5)));
                            a8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(a * 255 + 0.5)));
                        }
                        else {
                            r8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(Gamma.Compress(r) * 255 + 0.5)));
                            g8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(Gamma.Compress(g) * 255 + 0.5)));
                            b8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(Gamma.Compress(b) * 255 + 0.5)));
                            if (destinationAlpha) {
                                a8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(a * 255 + 0.5)));
                            }
                        }
                    }
                    else { // sRGB
                        if (premultiplied) {
                            r8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(Gamma.Compress_sRGB(r) * a * 255 + 0.5)));
                            g8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(Gamma.Compress_sRGB(g) * a * 255 + 0.5)));
                            b8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(Gamma.Compress_sRGB(b) * a * 255 + 0.5)));
                            a8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(a * 255 + 0.5)));
                        }
                        else {
                            r8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(Gamma.Compress_sRGB(r) * 255 + 0.5)));
                            g8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(Gamma.Compress_sRGB(g) * 255 + 0.5)));
                            b8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(Gamma.Compress_sRGB(b) * 255 + 0.5)));
                            if (destinationAlpha) {
                                a8 = (byte)Math.Min(255, Math.Max(0, Math.Floor(a * 255 + 0.5)));
                            }
                        }
                    }

                    if (bgra) {
                        buffer[destIndex] = b8;
                        buffer[destIndex + 1] = g8;
                        buffer[destIndex + 2] = r8;
                    }
                    else {
                        buffer[destIndex] = r8;
                        buffer[destIndex + 1] = g8;
                        buffer[destIndex + 2] = b8;
                    }
                    if (destinationAlpha) {
                        buffer[destIndex + 3] = a8;
                    }
                }
                else if (bitsPerPixel == 32) {
                    float r32, g32, b32, a32 = 1.0f;

                    if (gamma == GammaEncoding.Linear) {
                        if (premultiplied) {
                            r32 = r * a;
                            g32 = g * a;
                            b32 = b * a;
                            a32 = a;
                        }
                        else {
                            r32 = r;
                            g32 = g;
                            b32 = b;
                            if (destinationAlpha) {
                                a32 = a;
                            }
                        }
                    }
                    else if (gamma == GammaEncoding.Gamma) {
                        if (premultiplied) {
                            r32 = Gamma.Compress(r) * a;
                            g32 = Gamma.Compress(g) * a;
                            b32 = Gamma.Compress(b) * a;
                            a32 = a;
                        }
                        else {
                            r32 = Gamma.Compress(r);
                            g32 = Gamma.Compress(g);
                            b32 = Gamma.Compress(b);
                            if (destinationAlpha) {
                                a32 = a;
                            }
                        }
                    }
                    else { // sRGB
                        if (premultiplied) {
                            r32 = Gamma.Compress_sRGB(r) * a;
                            g32 = Gamma.Compress_sRGB(g) * a;
                            b32 = Gamma.Compress_sRGB(b) * a;
                            a32 = a;
                        }
                        else {
                            r32 = Gamma.Compress_sRGB(r);
                            g32 = Gamma.Compress_sRGB(g);
                            b32 = Gamma.Compress_sRGB(b);
                            if (destinationAlpha) {
                                a32 = a;
                            }
                        }
                    }

                    if (bgra) {
                        writer.Write(b32);
                        writer.Write(g32);
                        writer.Write(r32);
                    }
                    else {
                        writer.Write(r32);
                        writer.Write(g32);
                        writer.Write(b32);
                    }
                    if (destinationAlpha) {
                        writer.Write(a32);
                    }
                }
                else { // 16
                    Half r16, g16, b16, a16 = new Half(1.0f);

                    if (gamma == GammaEncoding.Linear) {
                        if (premultiplied) {
                            r16 = (Half)(r * a);
                            g16 = (Half)(g * a);
                            b16 = (Half)(b * a);
                            a16 = (Half)a;
                        }
                        else {
                            r16 = (Half)r;
                            g16 = (Half)g;
                            b16 = (Half)b;
                            if (destinationAlpha) {
                                a16 = (Half)a;
                            }
                        }
                    }
                    else if (gamma == GammaEncoding.Gamma) {
                        if (premultiplied) {
                            r16 = (Half)(Gamma.Compress(r) * a);
                            g16 = (Half)(Gamma.Compress(g) * a);
                            b16 = (Half)(Gamma.Compress(b) * a);
                            a16 = (Half)a;
                        }
                        else {
                            r16 = (Half)Gamma.Compress(r);
                            g16 = (Half)Gamma.Compress(g);
                            b16 = (Half)Gamma.Compress(b);
                            if (destinationAlpha) {
                                a16 = (Half)a;
                            }
                        }
                    }
                    else { // sRGB
                        if (premultiplied) {
                            r16 = (Half)(Gamma.Compress_sRGB(r) * a);
                            g16 = (Half)(Gamma.Compress_sRGB(g) * a);
                            b16 = (Half)(Gamma.Compress_sRGB(b) * a);
                            a16 = (Half)a;
                        }
                        else {
                            r16 = (Half)Gamma.Compress_sRGB(r);
                            g16 = (Half)Gamma.Compress_sRGB(g);
                            b16 = (Half)Gamma.Compress_sRGB(b);
                            if (destinationAlpha) {
                                a16 = (Half)a;
                            }
                        }
                    }

                    if (bgra) {
                        writer.Write(b16.value);
                        writer.Write(g16.value);
                        writer.Write(r16.value);
                    }
                    else {
                        writer.Write(r16.value);
                        writer.Write(g16.value);
                        writer.Write(b16.value);
                    }
                    if (destinationAlpha) {
                        writer.Write(a16.value);
                    }
                }
            }
        }

#if DOTNET
        public void Open(string file) {
            var reader = new EXRReader(new FileStream(file, FileMode.Open, FileAccess.Read));
            Open(reader);
            reader.Dispose();
        }
#endif

        public void Open(Stream stream) {
            var reader = new EXRReader(new BinaryReader(stream));
            Open(reader);
            reader.Dispose();
        }

        public void Close() {
            hasData = false;
            HalfChannels.Clear();
            FloatChannels.Clear();
        }

        public void Open(IEXRReader reader) {
            hasData = true;
            ReadPixelData(reader);
        }

        private void ReadPixelBlock(IEXRReader reader, uint offset, int linesPerBlock, List<Channel> sortedChannels) {
            reader.Position = (int)offset;

            if (Version.IsMultiPart) {
                // we don't use this. should we? i dunno. probably not
                reader.ReadUInt32(); reader.ReadUInt32();
            }

            var startY = reader.ReadInt32();
            var endY = Math.Min(DataWindow.Height, startY + linesPerBlock);
            var startIndex = startY * DataWindow.Width;

            var dataSize = reader.ReadInt32();

            if (Header.Compression != EXRCompression.None) {
                throw new NotImplementedException("Compressed images are currently not supported");
            }

            foreach (var channel in sortedChannels) {
                float[] floatArr = null;
                Half[] halfArr = null;

                if (channel.Type == PixelType.Float) {
                    floatArr = FloatChannels[channel.Name];
                }
                else if (channel.Type == PixelType.Half) {
                    halfArr = HalfChannels[channel.Name];
                }
                else {
                    throw new NotImplementedException();
                }

                var index = startIndex;
                for (int y = startY; y < endY; y++) {
                    for (int x = 0; x < DataWindow.Width; x++, index++) {
                        if (channel.Type == PixelType.Float) {
                            floatArr[index] = reader.ReadSingle();
                        }
                        else if (channel.Type == PixelType.Half) {
                            halfArr[index] = reader.ReadHalf();
                        }
                        else {
                            throw new NotImplementedException();
                        }
                    }
                }
            }
        }

#if PARALLEL
        public void OpenParallel(string file) {
            OpenParallel(() => {
                return new EXRReader(new FileStream(file, FileMode.Open, FileAccess.Read));
            });
        }

        public void OpenParallel(ParallelReaderCreationDelegate createReader) {
            hasData = true;
            ReadPixelDataParallel(createReader);
        }

        private void ReadPixelDataParallel(ParallelReaderCreationDelegate createReader) {
            var linesPerBlock = EXRFile.GetScanLinesPerBlock(Header.Compression);
            var sortedChannels = (from c in Header.Channels orderby c.Name select c).ToList();

            var actions = (from offset in Offsets select (Action)(() => {
                var reader = createReader();
                ReadPixelBlock(reader, offset, linesPerBlock, sortedChannels);
                reader.Dispose();
            }));
            Parallel.Invoke(actions.ToArray());
        }
#else
        public void OpenParallel(string file) {
            Open(file);
        }

        public void OpenParallel(ParallelReaderCreationDelegate createReader) {
            var reader = createReader();
            Open(reader);
            reader.Dispose();
        }
#endif

        protected void ReadPixelData(IEXRReader reader) {
            var linesPerBlock = EXRFile.GetScanLinesPerBlock(Header.Compression);
            var sortedChannels = (from c in Header.Channels orderby c.Name select c).ToList();

            //var actions = (from offset in Offsets select (Action)(() => {
            //}));
            //Parallel.Invoke(actions.ToArray());
            foreach (var offset in Offsets) {
                ReadPixelBlock(reader, offset, linesPerBlock, sortedChannels);
            }
        }

        public bool IsRGB {
            get {
                return
                    (HalfChannels.ContainsKey("R") || FloatChannels.ContainsKey("R")) &&
                    (HalfChannels.ContainsKey("G") || FloatChannels.ContainsKey("G")) &&
                    (HalfChannels.ContainsKey("B") || FloatChannels.ContainsKey("B"));
            }
        }

        public bool HasAlpha {
            get {
                return HalfChannels.ContainsKey("A") || FloatChannels.ContainsKey("A");
            }
        }
    }
}
