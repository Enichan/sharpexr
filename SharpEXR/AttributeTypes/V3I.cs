namespace SharpEXR.AttributeTypes
{
    public struct V3I
    {
        public int V0;
        public int V1;
        public int V2;

        public V3I(int v0, int v1, int v2)
        {
            V0 = v0;
            V1 = v1;
            V2 = v2;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}, {2}, {3}", GetType().Name, V0, V1, V2);
        }

        public int X => V0;
        public int Y => V1;
        public int Z => V2;
    }
}
