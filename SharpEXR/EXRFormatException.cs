using System;

namespace SharpEXR
{
    public sealed class EXRFormatException : Exception
    {
        public EXRFormatException()
            : base()
        {
        }
        public EXRFormatException(string message)
            : base(message)
        {
        }
        public EXRFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
