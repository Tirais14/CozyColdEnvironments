using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class SignatureComparer
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsMatch(Type value,
                                   ParameterInfo parameter,
                                   bool allowInheritance = false)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            if (parameter is null)
                throw new ArgumentNullException(nameof(parameter));

            if (value == parameter.ParameterType)
                return true;

            if (allowInheritance && value.IsType(parameter.ParameterType))
                return true;

            return false;
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsMatch(Type[] values,
                                   ParameterInfo[] parameters,
                                   bool allowInheritance = false,
                                   bool ignoreDefaultValues = false)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));

            if (values.IsEmpty() && parameters.IsEmpty())
                return true;

            if (values.IsEmpty())
                return false;

            if (ignoreDefaultValues)
                parameters = parameters.Where(x => !x.HasDefaultValue).ToArray();

            if (values.Length != parameters.Length)
                return false;

            int count = values.Length;
            if (allowInheritance)
            {
                for (int i = 0; i < count; i++)
                {
                    if (values[i].IsNotType(parameters[i].ParameterType))
                        return false;
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (values[i] != parameters[i].ParameterType)
                        return false;
                }
            }

            return true;
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsMatch(IEnumerable<Type> values,
                                   ParameterInfo[] parameters,
                                   bool allowInheritance = false,
                                   bool ignoreDefaultValues = false)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));

            return IsMatch(values.ToArray(),
                           parameters,
                           allowInheritance,
                           ignoreDefaultValues);
        }
    }
}
