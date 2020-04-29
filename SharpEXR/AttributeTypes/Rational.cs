namespace SharpEXR.AttributeTypes
{
    public readonly struct Rational
    {
        public readonly int Numerator;
        public readonly uint Denominator;

        public Rational(int numerator, uint denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public override string ToString()
        {
            return $"{Numerator}/{Denominator}";
        }

        public double Value => (double)Numerator / Denominator;
    }
}
