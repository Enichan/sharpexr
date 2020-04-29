using System;

namespace SharpEXR
{
    public interface IEXRReader : IDisposable
    {
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
}
