#nullable enable
namespace CCEnvs
{
    public static class Range
    {
        public static T[] From<T>(params T[] values)
        {
            CC.Validate.ArgumentNull(values, nameof(values));

            return values;
        }
    }
}
