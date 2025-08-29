using System;
using CozyColdEnvironments.Reflection;

#nullable enable

namespace CozyColdEnvironments.Diagnostics
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