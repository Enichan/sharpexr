using SharpEXR.AttributeTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR {
    public class EXRAttribute {
        public string Name { get; protected set; }
        public string Type { get; protected set; }
        public int Size { get; protected set; }
        public object Value { get; protected set; }

        public EXRAttribute() {
        }

        public static bool Read(EXRFile file, IEXRReader reader, out EXRAttribute attribute) {
            attribute = new EXRAttribute();
            return attribute.Read(file, reader);
        }

        public override string ToString() {
            return Value.ToString();
        }

        /// <summary>
        /// Returns true unless this is the end of the header
        /// </summary>
        public bool Read(EXRFile file, IEXRReader reader) {
            var maxLen = file.Version.MaxNameLength;

            try {
                Name = reader.ReadNullTerminatedString(maxLen);
            }
            catch (Exception e) {
                throw new EXRFormatException("Invalid or corrupt EXR header attribute name: " + e.Message, e);
            }
            if (Name == "") {
                return false;
            }

            try {
                Type = reader.ReadNullTerminatedString(maxLen);
            }
            catch (Exception e) {
                throw new EXRFormatException("Invalid or corrupt EXR header attribute type for '" + Name + "': " + e.Message, e);
            }
            if (Type == "") {
                throw new EXRFormatException("Invalid or corrupt EXR header attribute type for '" + Name + "': Cannot be an empty string.");
            }

            Size = reader.ReadInt32();

            switch (Type) {
                case "box2i":
                    if (Size != 16) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type box2i: Size must be 16 bytes, was " + Size + ".");
                    }
                    Value = new Box2I(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                    break;
                case "box2f":
                    if (Size != 16) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type box2f: Size must be 16 bytes, was " + Size + ".");
                    }
                    Value = new Box2F(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    break;
                case "chromaticities":
                    if (Size != 32) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type chromaticities: Size must be 32 bytes, was " + Size + ".");
                    }
                    Value = new Chromaticities(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    break;
                case "compression":
                    if (Size != 1) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type compression: Size must be 1 byte, was " + Size + ".");
                    }
                    Value = (EXRCompression)reader.ReadByte();
                    break;
                case "double":
                    if (Size != 8) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type double: Size must be 8 bytes, was " + Size + ".");
                    }
                    Value = reader.ReadDouble();
                    break;
                case "envmap":
                    if (Size != 1) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type envmap: Size must be 1 byte, was " + Size + ".");
                    }
                    Value = (EnvMap)reader.ReadByte();
                    break;
                case "float":
                    if (Size != 4) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type float: Size must be 4 bytes, was " + Size + ".");
                    }
                    Value = reader.ReadSingle();
                    break;
                case "int":
                    if (Size != 4) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type int: Size must be 4 bytes, was " + Size + ".");
                    }
                    Value = reader.ReadInt32();
                    break;
                case "keycode":
                    if (Size != 28) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type keycode: Size must be 28 bytes, was " + Size + ".");
                    }
                    Value = new KeyCode(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                    break;
                case "lineOrder":
                    if (Size != 1) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type lineOrder: Size must be 1 byte, was " + Size + ".");
                    }
                    Value = (LineOrder)reader.ReadByte();
                    break;
                case "m33f":
                    if (Size != 36) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type m33f: Size must be 36 bytes, was " + Size + ".");
                    }
                    Value = new M33F(
                        reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 
                        reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 
                        reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    break;
                case "m44f":
                    if (Size != 64) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type m44f: Size must be 64 bytes, was " + Size + ".");
                    }
                    Value = new M44F(
                        reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 
                        reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                        reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                        reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    break;
                case "rational":
                    if (Size != 8) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type rational: Size must be 8 bytes, was " + Size + ".");
                    }
                    Value = new Rational(reader.ReadInt32(), reader.ReadUInt32());
                    break;
                case "string":
                    if (Size < 0) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type string: Invalid Size, was " + Size + ".");
                    }
                    Value = reader.ReadString(Size);
                    break;
                case "stringvector":
                    if (Size == 0) {
                        Value = new List<string>();
                    }
                    else if (Size < 4) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type stringvector: Size must be at least 4 bytes or 0 bytes, was " + Size + ".");
                    }
                    else {
                        var strings = new List<string>();
                        Value = strings;
                        var bytesRead = 0;

                        while (bytesRead < Size) {
                            var loc = reader.Position;
                            var str = reader.ReadString();
                            strings.Add(str);
                            bytesRead += reader.Position - loc;
                        }

                        if (bytesRead != Size) {
                            throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type stringvector: Read " + bytesRead + " bytes but Size was " + Size + ".");
                        }
                    }
                    break;
                case "tiledesc":
                    if (Size != 9) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type tiledesc: Size must be 9 bytes, was " + Size + ".");
                    }
                    Value = new TileDesc(reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadByte());
                    break;
                case "timecode":
                    if (Size != 8) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type timecode: Size must be 8 bytes, was " + Size + ".");
                    }
                    Value = new TimeCode(reader.ReadUInt32(), reader.ReadUInt32());
                    break;
                case "v2i":
                    if (Size != 8) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type v2i: Size must be 8 bytes, was " + Size + ".");
                    }
                    Value = new V2I(reader.ReadInt32(), reader.ReadInt32());
                    break;
                case "v2f":
                    if (Size != 8) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type v2f: Size must be 8 bytes, was " + Size + ".");
                    }
                    Value = new V2F(reader.ReadSingle(), reader.ReadSingle());
                    break;
                case "v3i":
                    if (Size != 12) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type v3i: Size must be 12 bytes, was " + Size + ".");
                    }
                    Value = new V3I(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                    break;
                case "v3f":
                    if (Size != 12) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type v3f: Size must be 12 bytes, was " + Size + ".");
                    }
                    Value = new V3F(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    break;
                case "chlist":
                    var chlist = new ChannelList();
                    try {
                        chlist.Read(file, reader, Size);
                    }
                    catch (Exception e) {
                        throw new EXRFormatException("Invalid or corrupt EXR header attribute '" + Name + "' of type chlist: " + e.Message, e);
                    }
                    Value = chlist;
                    break;
                case "preview":
                default:
                    Value = reader.ReadBytes(Size);
                    break;
            }

            return true;
        }
    }
}
