﻿namespace SharpEXR.AttributeTypes
{
    public struct V3F
    {
        public float V0;
        public float V1;
        public float V2;

        public V3F(float v0, float v1, float v2)
        {
            V0 = v0;
            V1 = v1;
            V2 = v2;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}, {2}, {3}", GetType().Name, V0, V1, V2);
        }

        public float X => V0;
        public float Y => V1;
        public float Z => V2;
    }
}
