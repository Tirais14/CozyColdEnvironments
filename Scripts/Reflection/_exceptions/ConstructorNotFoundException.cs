using System;
using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Reflection.Diagnostics
{
    public class ConstructorNotFoundException : CCEException
    {
        public ConstructorNotFoundException()
        {
        }

        public ConstructorNotFoundException(Type type, ConstructorBindings parameters)
            : base($"Constructor not found in type {type.GetName()}. Parameters: {parameters}")
        {
        }
    }
}
