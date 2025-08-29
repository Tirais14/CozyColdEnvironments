using System;
using UTIRLib.Reflection;

#nullable enable

namespace UTIRLib.Diagnostics
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