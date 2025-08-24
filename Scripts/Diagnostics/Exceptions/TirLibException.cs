using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Analytics;
using UTIRLib.Diagnostics.Internal;
using UTIRLib.Reflection;

#nullable enable

#pragma warning disable S1751
namespace UTIRLib.Diagnostics
{
    public class TirLibException : Exception
    {
        private const string PARAM_NAME_MSG = "Parameter: {0}";

        public TirLibException() : base()
        {
        }

        public TirLibException(string message,
                               Exception? innerException = null)
            :
            base(message, innerException)
        {
        }

        public TirLibException(Exception? innerException,
                               string notFormattedMessage,
                               params object[] args)
            :
            base(string.Format(notFormattedMessage, args), innerException)
        {
        }

        public TirLibException(string notFormattedMessage,
                               params object[] args)
            :
            base(string.Format(notFormattedMessage, args))
        {
        }

        protected static string CombineToAssignmentMessage(
            params (string paramName, string value)[] args)
        {
            IEnumerable<string> converted = args.Select(x => x.paramName + " = " + x.value);

            return converted.JoinStrings(", ");
        }

        protected static string DefineParameterValue(object? value)
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

        protected static string ConstructMessage(Type thisType,
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
        protected static string ConstructMessage(Type thisType, params object[] args)
        {
            TypeValuePair[] infos = args.Select(x =>
            {
                if (x is TypeValuePair argInfo)
                    return argInfo;

                return new TypeValuePair(x.GetType(), x);
            }).ToArray();

            return ConstructMessage(thisType, infos!);
        }

        [Obsolete("Use CombineToAssignmentMessage instead.")]
        protected static string GetParamNameMsg(string paramName) => string.Format(PARAM_NAME_MSG, paramName);

        [Obsolete("Use GetType().GetName() instead.")]
        protected static string GetObjectTypeName(object? obj) => obj.IsNull() ? "null" : obj.GetType().Name;
    }
}