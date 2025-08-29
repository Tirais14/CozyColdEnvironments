using System;
using System.Reflection;
using CozyColdEnvironments.Diagnostics;

#nullable enable
namespace CozyColdEnvironments.Reflection
{
    public static class ConstructorBindingsMatcher
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsMatch(ConstructorBindings bindings, ConstructorInfo ctor)
        {
            if (bindings is null)
                throw new ArgumentNullException(nameof(bindings));
            if (ctor is null)
                throw new ArgumentNullException(nameof(ctor));

            if (bindings.Arguments.signature != ctor.GetSignature())
                return false;

            if (bindings.ParameterModifiersArray.IsNotDefault())
            {
                ParameterModifier ctorParamModifiers = ctor.GetParameterModifiers();
                if (!bindings.ParameterModifiers.Equals(ctorParamModifiers))
                    return false;
            }

            return true;
        }
    }
}
