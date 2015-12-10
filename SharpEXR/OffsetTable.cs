using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR {
    public class OffsetTable : IEnumerable<uint> {
        public List<uint> Offsets { get; set; }

        public OffsetTable() {
            Offsets = new List<uint>();
        }

        public OffsetTable(int capacity) {
            Offsets = new List<uint>(capacity);
        }

        public void Read(IEXRReader reader, int count) {
            for (int i = 0; i < count; i++) {
                Offsets.Add(reader.ReadUInt32());
                reader.ReadUInt32(); // skip 4 bytes because we're using uints not ulongs
            }
        }

        public IEnumerator<uint> GetEnumerator() {
            return Offsets.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
