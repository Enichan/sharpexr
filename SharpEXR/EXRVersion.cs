using System;

namespace SharpEXR
{
    [Flags]
    public enum EXRVersionFlags
    {
        IsSinglePartTiled = 0x200,
        LongNames = 0x400,
        NonImageParts = 0x800,
        MultiPart = 0x1000
    }

    public readonly struct EXRVersion
    {
        public readonly EXRVersionFlags Value;

        public EXRVersion(int version, bool multiPart, bool longNames, bool nonImageParts, bool isSingleTiled = false)
        {
            Value = (EXRVersionFlags)(version & 0xFF);

            if (version == 1)
            {
                if (multiPart || nonImageParts)
                {
                    throw new EXRFormatException("Invalid or corrupt EXR version: Version 1 EXR files cannot be multi part or have non image parts.");
                }
                if (isSingleTiled)
                {
                    Value |= EXRVersionFlags.IsSinglePartTiled;
                }
                if (longNames)
                {
                    Value |= EXRVersionFlags.LongNames;
                }
            }
            else
            {
                if (isSingleTiled)
                {
                    Value |= EXRVersionFlags.IsSinglePartTiled;
                }
                if (longNames)
                {
                    Value |= EXRVersionFlags.LongNames;
                }
                if (nonImageParts)
                {
                    Value |= EXRVersionFlags.NonImageParts;
                }
                if (multiPart)
                {
                    Value |= EXRVersionFlags.MultiPart;
                }
            }

            Verify();
        }

        public EXRVersion(int value)
        {
            Value = (EXRVersionFlags)value;
            Verify();
        }

        private void Verify()
        {
            if (IsSinglePartTiled)
            {
                // below bits must be 0
                if (IsMultiPart || HasNonImageParts)
                {
                    throw new EXRFormatException("Invalid or corrupt EXR version: Version's single part bit was set, but multi part and/or non image data bits were also set.");
                }
            }
        }

        public int Version => (int)Value & 0xFF;

        public bool IsSinglePartTiled => Value.HasFlag(EXRVersionFlags.IsSinglePartTiled);

        public bool HasLongNames => Value.HasFlag(EXRVersionFlags.LongNames);

        public bool HasNonImageParts => Value.HasFlag(EXRVersionFlags.NonImageParts);

        public bool IsMultiPart => Value.HasFlag(EXRVersionFlags.MultiPart);

        public int MaxNameLength => HasLongNames ? 255 : 31;
    }
}
