using CCEnvs.Diagnostics;
using CCEnvs.Reflection.Data;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class ConstructorBindingsMatcher
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsMatch(MethodBindings bindings, ConstructorInfo ctor)
        {
            if (bindings is null)
                throw new ArgumentNullException(nameof(bindings));
            if (ctor is null)
                throw new ArgumentNullException(nameof(ctor));

            if (((CCParameters)bindings.Arguments) != ctor.GetCCParameters())
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
