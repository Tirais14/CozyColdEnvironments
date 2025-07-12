using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UTIRLib.Attributes;
using UTIRLib.Extensions;

#nullable enable

namespace UTIRLib.Initializer
{
    public readonly struct InitializeMethodInfo
    {
        private readonly ParameterInfo[] parameters;
        public readonly object source;
        public readonly MethodInfo method;
        public readonly InitAttribute attribute;

        public readonly IReadOnlyList<ParameterInfo> Parameters => parameters;

        public InitializeMethodInfo(object source, MethodInfo method, InitAttribute attribute)
        {
            this.source = source;
            this.method = method;
            parameters = method.GetParameters() ?? Array.Empty<ParameterInfo>();
            parameters = parameters.OrderBy((parameter) => parameter.Position).ToArray();
            this.attribute = attribute;
        }

        public void Invoke(object?[]? args = null)
        {
            if (parameters.IsNullOrEmpty())
            {
                method.Invoke(source, Array.Empty<object>());
            }
            else
            {
                method.Invoke(source, ResolveParameters(args!));
            }
        }

        public object[] ResolveParameters(object[] args)
        {
            if (args.Length != parameters.Length)
            {
                throw new ArgumentException(
                    $"Wrong number of parameters. Expected: {parameters.Length}, actual: {args.Length}");
            }

            var resolvedArgs = new object[parameters.Length];
            var usedArgs = new bool[args.Length];

            for (int paramIndex = 0; paramIndex < parameters.Length; paramIndex++)
            {
                var paramType = parameters[paramIndex].ParameterType;
                bool found = false;

                for (int argIndex = 0; argIndex < args.Length; argIndex++)
                {
                    if (!usedArgs[argIndex] && (args[argIndex] == null || paramType.IsInstanceOfType(args[argIndex])))
                    {
                        resolvedArgs[paramIndex] = args[argIndex];
                        usedArgs[argIndex] = true;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new ArgumentException(
                        $"No matching argument found for parameter {parameters[paramIndex].Name} of type {paramType}");
                }
            }

            return resolvedArgs;
        }
    }
}