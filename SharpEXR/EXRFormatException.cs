using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR {
    public class EXRFormatException : Exception {
        public EXRFormatException()
            : base() {
        }
        public EXRFormatException(string message)
            : base(message) {
        }
        public EXRFormatException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }
}
