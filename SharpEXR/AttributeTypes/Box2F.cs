namespace SharpEXR.AttributeTypes
{
    public readonly struct Box2F
    {
        public readonly float XMin;
        public readonly float YMin;
        public readonly float XMax;
        public readonly float YMax;

        public Box2F(float xMin, float yMin, float xMax, float yMax)
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

        public float Width => (XMax - XMin) + 1;

        public float Height => (YMax - YMin) + 1;
    }
}
