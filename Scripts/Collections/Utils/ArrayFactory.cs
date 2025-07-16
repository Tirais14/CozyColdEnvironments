#nullable enable
namespace UTIRLib.Collections
{
    public static class ArrayFactory
    {
        public static T[] Create<T>(params T[] values) => values;
    }
}
