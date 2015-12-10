using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR {
    public enum ChannelConfiguration {
        /// <summary>
        /// First byte is blue, then green, then red, then (optionally) alpha
        /// </summary>
        BGR,
        /// <summary>
        /// First byte is red, then green, then blue, then (optionally) alpha
        /// </summary>
        RGB
    }
}
