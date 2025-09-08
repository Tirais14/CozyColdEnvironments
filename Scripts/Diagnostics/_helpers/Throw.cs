#nullable enable
using CCEnvs.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Diagnostics
{
    public static class Throw
    {
        [DoesNotReturn]
        public static void InvalidCast(Type toType)
        {
            throw new InvalidCastException($"Conversation type = {toType.GetFullName()}.");
        }

        [DoesNotReturn]
        public static void InvalidCast(Type fromType, Type toType)
        {
            throw new InvalidCastException($"From {fromType.GetFullName()} to {toType.GetFullName()}.");
        }
    }
}
