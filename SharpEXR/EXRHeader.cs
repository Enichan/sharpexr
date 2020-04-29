using System.Collections.Generic;
using SharpEXR.AttributeTypes;

namespace SharpEXR
{
    public sealed class EXRHeader
    {
        public static readonly Chromaticities DefaultChromaticities = new Chromaticities(
            0.6400f, 0.3300f,
            0.3000f, 0.6000f,
            0.1500f, 0.0600f,
            0.3127f, 0.3290f
        );

        public Dictionary<string, EXRAttribute> Attributes { get; private set; }

        public EXRHeader()
        {
            Attributes = new Dictionary<string, EXRAttribute>();
        }

        public void Read(EXRFile file, IEXRReader reader)
        {
            while (EXRAttribute.Read(file, reader, out EXRAttribute attribute))
            {
                Attributes[attribute.Name] = attribute;
            }
        }

        public bool TryGetAttribute<T>(string name, out T result)
        {
            if (!Attributes.TryGetValue(name, out EXRAttribute attr))
            {
                result = default;
                return false;
            }

            if (attr.Value == null)
            {
                result = default; // assign default value, which will be null for non-value types
                // return true if not a value type
                return !typeof(T).IsClass && !typeof(T).IsInterface && !typeof(T).IsArray;
            }
            else
            {
                if (typeof(T).IsAssignableFrom(attr.Value.GetType()))
                {
                    result = (T)attr.Value;
                    return true;
                }
                else
                {
                    result = default;
                    return false;
                }
            }
        }

        public bool IsEmpty { get { return Attributes.Count == 0; } }

        public int ChunkCount
        {
            get
            {
                if (!TryGetAttribute("chunkCount", out int chunkCount))
                {
                    throw new EXRFormatException("Invalid or corrupt EXR header: Missing chunkCount attribute.");
                }

                return chunkCount;
            }
        }

        public Box2I DataWindow
        {
            get
            {
                if (!TryGetAttribute("dataWindow", out Box2I dataWindow))
                {
                    throw new EXRFormatException("Invalid or corrupt EXR header: Missing dataWindow attribute.");
                }

                return dataWindow;
            }
        }

        public EXRCompression Compression
        {
            get
            {
                if (!TryGetAttribute("compression", out EXRCompression compression))
                {
                    throw new EXRFormatException("Invalid or corrupt EXR header: Missing compression attribute.");
                }
                return compression;
            }
        }

        public PartType Type
        {
            get
            {
                if (!TryGetAttribute("type", out PartType type))
                {
                    throw new EXRFormatException("Invalid or corrupt EXR header: Missing type attribute.");
                }

                return type;
            }
        }

        public ChannelList Channels
        {
            get
            {
                if (!TryGetAttribute("channels", out ChannelList channels))
                {
                    throw new EXRFormatException("Invalid or corrupt EXR header: Missing channels attribute.");
                }

                return channels;
            }
        }

        // TODO: Should we search like this? Spec is unclear
        public Chromaticities Chromaticities
        {
            get
            {
                foreach (var attr in Attributes.Values)
                {
                    if (attr.Type == "chromaticities" && attr.Value is Chromaticities)
                    {
                        return (Chromaticities)attr.Value;
                    }
                }
                return DefaultChromaticities;
            }
        }
    }
}
