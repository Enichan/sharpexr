using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR {
    public class Channel {
        public string Name { get; set; }
        public PixelType Type { get; set; }
        public bool Linear { get; set; }
        public int XSampling { get; set; }
        public int YSampling { get; set; }

        public byte[] Reserved { get; set; }

        public Channel(string name, PixelType type, bool linear, int xSampling, int ySampling)
            : this(name, type, linear, 0, 0, 0, xSampling, ySampling) {
        }

        public Channel(string name, PixelType type, bool linear, byte reserved0, byte reserved1, byte reserved2, int xSampling, int ySampling) {
            Name = name;
            Type = type;
            Linear = linear;
            Reserved = new byte[3] { reserved0, reserved1, reserved2 };
        }

        public override string ToString() {
            return string.Format("{0} {1} {2}", GetType().Name, Name, Type);
        }
    }
}
