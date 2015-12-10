using SharpEXR.AttributeTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEXR {
    public class EXRHeader {
        public static readonly Chromaticities DefaultChromaticities = new Chromaticities(
            0.6400f, 0.3300f,
            0.3000f, 0.6000f,
            0.1500f, 0.0600f,
            0.3127f, 0.3290f
        );

        public Dictionary<string, EXRAttribute> Attributes { get; protected set; }

        public EXRHeader() {
            Attributes = new Dictionary<string, EXRAttribute>();
        }

        public void Read(EXRFile file, IEXRReader reader) {
            EXRAttribute attribute;
            while (EXRAttribute.Read(file, reader, out attribute)) {
                Attributes[attribute.Name] = attribute;
            }
        }

        public bool TryGetAttribute<T>(string name, out T result) {
            EXRAttribute attr;
            if (!Attributes.TryGetValue(name, out attr)) {
                result = default(T);
                return false;
            }

            if (attr.Value == null) {
                result = default(T); // assign default value, which will be null for non-value types
                // return true if not a value type
                return !typeof(T).IsClass && !typeof(T).IsInterface && !typeof(T).IsArray;
            }
            else {
                if (typeof(T).IsAssignableFrom(attr.Value.GetType())) {
                    result = (T)attr.Value;
                    return true;
                }
                else {
                    result = default(T);
                    return false;
                }
            }
        }

        public bool IsEmpty { get { return Attributes.Count == 0; } }

        public int ChunkCount {
            get {
                int chunkCount;
                if (!TryGetAttribute<int>("chunkCount", out chunkCount)) {
                    throw new EXRFormatException("Invalid or corrupt EXR header: Missing chunkCount attribute.");
                }
                return chunkCount;
            }
        }

        public Box2I DataWindow {
            get {
                Box2I dataWindow;
                if (!TryGetAttribute<Box2I>("dataWindow", out dataWindow)) {
                    throw new EXRFormatException("Invalid or corrupt EXR header: Missing dataWindow attribute.");
                }
                return dataWindow;
            }
        }

        public EXRCompression Compression {
            get {
                EXRCompression compression;
                if (!TryGetAttribute<EXRCompression>("compression", out compression)) {
                    throw new EXRFormatException("Invalid or corrupt EXR header: Missing compression attribute.");
                }
                return compression;
            }
        }

        public PartType Type {
            get {
                PartType type;
                if (!TryGetAttribute<PartType>("type", out type)) {
                    throw new EXRFormatException("Invalid or corrupt EXR header: Missing type attribute.");
                }
                return type;
            }
        }

        public ChannelList Channels {
            get {
                ChannelList channels;
                if (!TryGetAttribute<ChannelList>("channels", out channels)) {
                    throw new EXRFormatException("Invalid or corrupt EXR header: Missing channels attribute.");
                }
                return channels;
            }
        }

        // TODO: Should we search like this? Spec is unclear
        public Chromaticities Chromaticities {
            get {
                foreach (var attr in Attributes.Values) {
                    if (attr.Type == "chromaticities" && attr.Value is Chromaticities) {
                        return (Chromaticities)attr.Value;
                    }
                }
                return DefaultChromaticities;
            }
        }
    }
}
