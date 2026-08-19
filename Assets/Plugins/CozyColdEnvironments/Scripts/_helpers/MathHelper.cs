#nullable enable
namespace CCEnvs
{
    public static class MathHelper
    {
        public static int FloorDiv(int value, int divider)
        {
            int quotient = value / divider;
            int remainder = value % divider;

            if (remainder != 0 && value < 0)
                quotient--;

            return quotient;
        }
    }
}
