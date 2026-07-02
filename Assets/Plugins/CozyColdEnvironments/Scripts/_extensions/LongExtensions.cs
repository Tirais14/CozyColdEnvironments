using System;

namespace CCEnvs
{
    public static class LongExtensions
    {
        public static int ToInt(this long source)
        {
            return (int)Math.Min(source, int.MaxValue);
        }
    }
}
