namespace SharpEXR.AttributeTypes
{
    public readonly struct TimeCode
    {
        public readonly uint TimeAndFlags;
        public readonly uint UserData;

        public TimeCode(uint timeAndFlags, uint userData)
        {
            TimeAndFlags = timeAndFlags;
            UserData = userData;
        }
    }
}
