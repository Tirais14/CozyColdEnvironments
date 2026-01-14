#nullable enable
using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using System;
using System.Linq;
using System.Reflection;

namespace CCEnvs.Reflection
{
    public static class MethodHelper
    {
        public static bool CompareParameters(this MethodBase methodBase,
                                             Type[] inputTypes,
                                             ParameterModifier? inputMods,
                                             out ParameterInfo[] parameters)
        {
            Guard.IsNotNull(methodBase, nameof(methodBase));
            Guard.IsNotNull(inputTypes, nameof(inputTypes));

            parameters = methodBase.GetParameters();

            if (parameters.Length != inputTypes.Length
                ||
                parameters.Zip(inputTypes, (param, other) => param.ParameterType.IsType(other)).Any(x => !x)
                )
            {
                return false;
            }

            ParameterModifier otherMods = parameters.GetParameterModifiers();
            for (int i = 0; i < parameters.Length; i++)
            {
                if ((inputMods is null
                    &&
                    !otherMods[i])
                    ||
                    (inputMods.IsNotNull()
                    &&
                    inputMods.Value[i] != otherMods[i])
                    )
                    return false;
            }

            return true;
        }
        public static bool CompareParameters(this MethodBase methodBase,
                                             Type[] inputTypes,
                                             ParameterModifier? inputMods)
        {
            return CompareParameters(methodBase, inputTypes, inputMods, out _);
        }
    }
}
