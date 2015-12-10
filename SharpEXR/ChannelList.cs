using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR {
    public class ChannelList : IEnumerable<Channel> {
        public List<Channel> Channels { get; set; }

        public ChannelList() {
            Channels = new List<Channel>();
        }

        public void Read(EXRFile file, IEXRReader reader, int size) {
            var totalSize = 0;
            Channel channel;
            int bytesRead;

            while (ReadChannel(file, reader, out channel, out bytesRead)) {
                Channels.Add(channel);
                totalSize += bytesRead;

                if (totalSize > size) {
                    throw new EXRFormatException("Read " + totalSize + " bytes but Size was " + size + ".");
                }
            }
            totalSize += bytesRead;

            if (totalSize != size) {
                throw new EXRFormatException("Read " + totalSize + " bytes but Size was " + size + ".");
            }
        }

        private bool ReadChannel(EXRFile file, IEXRReader reader, out Channel channel, out int bytesRead) {
            var start = reader.Position;

            var name = reader.ReadNullTerminatedString(255);
            if (name == "") {
                channel = null;
                bytesRead = reader.Position - start;
                return false;
            }

            channel = new Channel(
                name,
                (PixelType)reader.ReadInt32(),
                reader.ReadByte() != 0,
                reader.ReadByte(), reader.ReadByte(), reader.ReadByte(),
                reader.ReadInt32(), reader.ReadInt32());

            bytesRead = reader.Position - start;
            return true;
        }

        public IEnumerator<Channel> GetEnumerator() {
            return Channels.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public Channel this[int index] { get { return Channels[index]; } set { Channels[index] = value; } }
    }
}
