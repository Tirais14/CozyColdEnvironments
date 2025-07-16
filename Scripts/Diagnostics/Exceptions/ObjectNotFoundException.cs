using System;

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

        public ObjectNotFoundException(Type objType) : base("Unity object {0} not found.", objType)
        {
        }
    }
}