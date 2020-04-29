namespace SharpEXR.AttributeTypes
{
    public readonly struct Box2I
    {
        public readonly int XMin;
        public readonly int YMin;
        public readonly int XMax;
        public readonly int YMax;

        public Box2I(int xMin, int yMin, int xMax, int yMax)
        {
            XMin = xMin;
            YMin = yMin;
            XMax = xMax;
            YMax = yMax;
        }

        public override string ToString()
        {
            return string.Format("{0}: ({1}, {2})-({3}, {4})", GetType().Name, XMin, YMin, XMax, YMax);
        }

        public int Width => (XMax - XMin) + 1;
        public int Height => (YMax - YMin) + 1;
    }
}
