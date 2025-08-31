using CCEnvs.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Diagnostics
{
    public static class ExceptionMessageConstructor
    {
        public static string CombineToAssignmentMessage(
            params (string paramName, string value)[] args)
        {
            IEnumerable<string> converted = args.Select(x => x.paramName + " = " + x.value);

            return converted.JoinStrings(", ");
        }

        public static string DefineParameterValue(object? value)
        {
            if (value is null)
                return "null";
            if (value.IsDefault())
                return "default";
            if (value is string str)
            {
                if (str.IsEmpty())
                    return "empty string";
                if (str.IsNullOrWhiteSpace())
                    return "whitespace string";

                return str;
            }
            if (value is IEnumerable enumerable)
            {
                bool isEmpty = true;
                foreach (var item in enumerable)
                {
                    isEmpty = false;
                    break;
                }

                if (isEmpty)
                    return "empty collection";
            }
            if (value is Type type)
                return type.GetName();

            return value.ToString();
        }

        public static string ConstructMessage(Type thisType,
            params TypeValuePair[] args)
        {
            ConstructorInfo ctor = thisType.GetConstructor(
                BindingFlagsDefault.InstanceAll,
                binder: null,
                args.Select(x => x.type).ToArray(),
                Array.Empty<ParameterModifier>())
                ??
                throw new NullReferenceException($"Cannot find constructor in type = {thisType.GetName()}");

            ParameterInfo[] parameters = ctor.GetParameters();
            var converted = new List<(string, string)>(args.Length);
            for (int i = 0; i < args.Length; i++)
                converted.Add((parameters[i].Name, DefineParameterValue(args[i].value)));

            return CombineToAssignmentMessage(converted.ToArray());
        }
        public static string ConstructMessage(Type thisType, params object[] args)
        {
            TypeValuePair[] infos = args.Select(x =>
            {
                if (x is TypeValuePair argInfo)
                    return argInfo;

                return new TypeValuePair(x.GetType(), x);
            }).ToArray();

            return ConstructMessage(thisType, infos!);
        }
        public static string ConstructMessage<T>(params TypeValuePair[] args)
            where T : Exception
        {
            return ConstructMessage(typeof(T), args);
        }
        public static string ConstructMessage<T>(params object[] args)
            where T : Exception
        {
            return ConstructMessage(typeof(T), args);
        }
    }
}
