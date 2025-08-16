using System;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Reflection.Diagnostics
{
    public class ConstructorNotFoundException : TirLibException
    {
        public ConstructorNotFoundException()
        {
        }

        public ConstructorNotFoundException(Type type, ConstructorParameters parameters)
            : base($"Constructor not found in type {type.GetName()}. Parameters: {parameters}")
        {
        }
    }
}
