using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System;

#nullable enable

namespace CCEnvs.Unity.Diagnostics
{
    public sealed class ObjectNotFoundException : CCEException
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