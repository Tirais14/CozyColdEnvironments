using System;
using CozyColdEnvironments.Diagnostics;

#nullable enable
namespace CozyColdEnvironments.Reflection.Diagnostics
{
    public class ConstructorNotFoundException : TirLibException
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
