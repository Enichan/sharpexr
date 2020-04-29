using System.Collections.Generic;

namespace SharpEXR
{
    public sealed class OffsetTable : IEnumerable<uint>
    {
        public List<uint> Offsets { get; set; }

        public OffsetTable()
        {
            Offsets = new List<uint>();
        }

        public OffsetTable(int capacity)
        {
            Offsets = new List<uint>(capacity);
        }

        public void Read(IEXRReader reader, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Offsets.Add(reader.ReadUInt32());
                reader.ReadUInt32(); // skip 4 bytes because we're using uints not ulongs
            }
        }

        public IEnumerator<uint> GetEnumerator() => Offsets.GetEnumerator();
       
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
