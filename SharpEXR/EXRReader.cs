using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR {
    public interface IEXRReader : IDisposable {
        byte ReadByte();
        int ReadInt32();
        uint ReadUInt32();
        Half ReadHalf();
        float ReadSingle();
        double ReadDouble();
        string ReadNullTerminatedString(int maxLength);
        string ReadString(int length);
        string ReadString();
        byte[] ReadBytes(int count);
        void CopyBytes(byte[] dest, int offset, int count);
        int Position { get; set; }
    }

    public class EXRReader : IDisposable, IEXRReader {
        private BinaryReader reader;

        public EXRReader(Stream stream, bool leaveOpen = false)
            : this(new BinaryReader(stream, Encoding.ASCII, leaveOpen)) {
        }

        public EXRReader(BinaryReader reader) {
            this.reader = reader;
        }

        public byte ReadByte() {
            return reader.ReadByte();
        }

        public int ReadInt32() {
            return reader.ReadInt32();
        }

        public uint ReadUInt32() {
            return reader.ReadUInt32();
        }

        public Half ReadHalf() {
            return Half.ToHalf(reader.ReadUInt16());
        }

        public float ReadSingle() {
            return reader.ReadSingle();
        }

        public double ReadDouble() {
            return reader.ReadDouble();
        }

        public string ReadNullTerminatedString(int maxLength) {
            var start = reader.BaseStream.Position;
            StringBuilder str = new StringBuilder();
            byte b;
            while ((b = reader.ReadByte()) != 0) {
                if (reader.BaseStream.Position - start > maxLength) {
                    throw new EXRFormatException("Null terminated string exceeded maximum length of " + maxLength + " bytes.");
                }
                str.Append((char)b);
            }
            return str.ToString();
        }

        public string ReadString() {
            var len = ReadInt32();
            return ReadString(len);
        }

        public string ReadString(int length) {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < length; i++) {
                str.Append((char)reader.ReadByte());
            }
            return str.ToString();
        }

        public byte[] ReadBytes(int count) {
            return reader.ReadBytes(count);
        }

        public void CopyBytes(byte[] dest, int offset, int count) {
            int bytesRead = reader.BaseStream.Read(dest, offset, count);
            if (bytesRead != count) {
                throw new Exception("Less bytes read than expected");
            }
        }

        #region IDisposable
        private bool disposed = false;

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposed)
                return;

            if (disposing) {
                // Free any other managed objects here.
                try {
                    reader.Dispose();
                }
                catch { }
            }

            // Free any unmanaged objects here.
            disposed = true;
        }

#if DOTNET
        ~EXRReader() {
            Dispose(false);
        }
#endif
        #endregion

        public int Position {
            get {
                return (int)reader.BaseStream.Position;
            }
            set {
                reader.BaseStream.Seek(value, System.IO.SeekOrigin.Begin);
            }
        }
    }
}
