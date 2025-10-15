using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace CCEnvs.Unity.Diagnostics
{
    public sealed class ObjectNotFoundException : CCException
    {
        public ObjectNotFoundException() : base()
        {
        }

        public ObjectNotFoundException(string message) : base(message)
        {
        }

        public ObjectNotFoundException(Type objType) : base($"Unity object {objType.GetName()} not found.")
        {
        }

        public static void ThrowIfDefault<T>([NotNull] T obj)
        {
            if (obj.IsDefault())
                throw new ObjectNotFoundException(typeof(T));
        }
    }
}