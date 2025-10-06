#nullable enable
namespace CCEnvs.Diagnostics
{
    public static class ObjectExtensions
    {
        public static DebugContext AsDebugContext(this object? value,
                                          DebugArguments arguments = default)
        {
            return new DebugContext(value, arguments);
        }
    }
}
