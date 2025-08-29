using System;
using CCEnvs.Reflection;

#nullable enable

namespace CCEnvs.Diagnostics
{
    public sealed class ObjectNotFoundException : TirLibException
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
    }
}