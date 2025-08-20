using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Analytics;

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

        protected static string DefineParameterValue(object value)
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

            return value.ToString();
        }

        protected static string ConstructMessage(Type thisType, params object?[] args)
        {
            ConstructorInfo ctor = thisType.GetConstructor(
                BindingFlagsDefault.InstancePublic,
                binder: null,
                new Type[] { thisType },
                Array.Empty<ParameterModifier>());

            ParameterInfo[] parameters = ctor.GetParameters();
            var converted = new List<(string, string)>(args.Length);
            for (int i = 0; i < args.Length; i++)
                converted.Add((parameters[i].Name, DefineParameterValue(args)));

            return CombineToAssignmentMessage(converted.ToArray());
        }

        [Obsolete("Use CombineToAssignmentMessage instead.")]
        protected static string GetParamNameMsg(string paramName) => string.Format(PARAM_NAME_MSG, paramName);

        [Obsolete("Use GetType().GetName() instead.")]
        protected static string GetObjectTypeName(object? obj) => obj.IsNull() ? "null" : obj.GetType().Name;
    }
}