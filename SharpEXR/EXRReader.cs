using System;
using System.IO;
using System.Text;

namespace SharpEXR
{
    public sealed class EXRReader : IDisposable, IEXRReader
    {
        private readonly BinaryReader reader;

        public EXRReader(Stream stream, bool leaveOpen = false)
            : this(new BinaryReader(stream, Encoding.ASCII, leaveOpen))
        {
        }

        public EXRReader(BinaryReader reader)
        {
            this.reader = reader;
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public int ReadInt32()
        {
            return reader.ReadInt32();
        }

        public uint ReadUInt32()
        {
            return reader.ReadUInt32();
        }

        public Half ReadHalf()
        {
            return Half.ToHalf(reader.ReadUInt16());
        }

        public float ReadSingle()
        {
            return reader.ReadSingle();
        }

        public double ReadDouble()
        {
            return reader.ReadDouble();
        }

        public string ReadNullTerminatedString(int maxLength)
        {
            var start = reader.BaseStream.Position;
            StringBuilder str = new StringBuilder();
            byte b;
            while ((b = reader.ReadByte()) != 0)
            {
                if (reader.BaseStream.Position - start > maxLength)
                {
                    throw new EXRFormatException("Null terminated string exceeded maximum length of " + maxLength + " bytes.");
                }
                str.Append((char)b);
            }
            return str.ToString();
        }

        public string ReadString()
        {
            var len = ReadInt32();
            return ReadString(len);
        }

        public string ReadString(int length)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                str.Append((char)reader.ReadByte());
            }
            return str.ToString();
        }

        public byte[] ReadBytes(int count)
        {
            return reader.ReadBytes(count);
        }

        public void CopyBytes(byte[] dest, int offset, int count)
        {
            int bytesRead = reader.BaseStream.Read(dest, offset, count);
            if (bytesRead != count)
            {
                throw new Exception("Less bytes read than expected");
            }
        }

        #region IDisposable

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                try
                {
                    reader.Dispose();
                }
                catch { }
            }

            // Free any unmanaged objects here.
            disposed = true;
        }

        ~EXRReader()
        {
            Dispose(false);
        }

        #endregion

        public int Position
        {
            get => (int)reader.BaseStream.Position;
            set => reader.BaseStream.Seek(value, SeekOrigin.Begin);
        }
    }
}
