using System;
using System.Reflection;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib
{
    public static class MemberInfoHelper
    {
        /// <exception cref="WrongCollectionException"></exception>
        public static object[] ResolveArguments(Type[]? memberArgumentTypes, object[] args, bool throwWhenMissing = true)
        {
            if (memberArgumentTypes.IsNull())
            {
                return Array.Empty<object>();
            }
            if (args.IsNullOrEmpty() && throwWhenMissing)
            {
                throw new CollectionArgumentException(nameof(args), args);
            }

            TryResolveArgumentsInternal(memberArgumentTypes, args,
                throwWhenMissing ? ArgumentsResolveRule.ThrowIfMissing : ArgumentsResolveRule.DefaultWhenMissing,
                out object[] resolvedArgs);

            return resolvedArgs;
        }

        /// <exception cref="WrongCollectionException"></exception>
        public static object[] ResolveArguments(ParameterInfo[]? memberParameters, object[] args, bool throwWhenMissing = true)
        {
            if (memberParameters.IsNull())
            {
                return Array.Empty<object>();
            }
            if (args.IsNullOrEmpty() && throwWhenMissing)
            {
                throw new CollectionArgumentException(nameof(args), args);
            }
            (Type[] methodArgumentTypes, object[] defaultArgumentValues) = DeconstructParameters(memberParameters);

            TryResolveArgumentsInternal(methodArgumentTypes, args,
                throwWhenMissing ? ArgumentsResolveRule.ThrowIfMissing : ArgumentsResolveRule.DefaultWhenMissing,
                out object[] resolvedArgs, defaultArgumentValues);

            return resolvedArgs;
        }

        public static ResolvedMemberInvokable ResolveArguments(ConstructorInfo constructor, object[] args,
            bool throwWhenMissing = true) =>
            new(constructor, ResolveArguments(constructor.GetParameters(), args, throwWhenMissing));

        public static ResolvedMemberInvokable ResolveArguments(MethodInfo method, object[] args,
            bool throwWhenMissing = true) =>
            new(method, ResolveArguments(method.GetParameters(), args, throwWhenMissing));

        public static object[] ResolveArguments(PropertyInfo property, object[] args, bool throwWhenMissing = true) =>
            ResolveArguments(new Type[] { property.PropertyType }, args, throwWhenMissing);

        public static object[] ResolveArguments(FieldInfo field, object[] args, bool throwWhenMissing = true) =>
            ResolveArguments(new Type[] { field.FieldType }, args, throwWhenMissing);

        public static bool TryResolveArguments(ConstructorInfo constructor, object[] args,
            out ResolvedMemberInvokable resolvedMember)
        {
            try
            {
                resolvedMember = ResolveArguments(constructor, args, throwWhenMissing: true);
            }
            catch (Exception)
            {
                resolvedMember = default;
            }

            return resolvedMember.IsNotDefault();
        }

        public static bool TryResolveArguments(MethodInfo method, object[] args,
            out ResolvedMemberInvokable resolvedMember)
        {
            try
            {
                resolvedMember = ResolveArguments(method, args, throwWhenMissing: true);
            }
            catch (Exception)
            {
                resolvedMember = default;
            }

            return resolvedMember.IsNotDefault();
        }

        public static bool TryResolveArguments(PropertyInfo propertyInfo, object[] args, out object[] resolvedArgs)
        {
            try
            {
                resolvedArgs = ResolveArguments(propertyInfo, args, throwWhenMissing: true);
            }
            catch (Exception)
            {
                resolvedArgs = Array.Empty<object>();
                return false;
            }

            return resolvedArgs.Length > 0;
        }

        public static bool TryResolveArguments(FieldInfo field, object[] args, out object[] resolvedArgs)
        {
            try
            {
                resolvedArgs = ResolveArguments(field, args, throwWhenMissing: true);
            }
            catch (Exception)
            {
                resolvedArgs = Array.Empty<object>();
                return false;
            }

            return resolvedArgs.Length > 0;
        }

        private static (Type[] memberArgumentTypes, object[] memberArgumentDefaultValues) DeconstructParameters(
    ParameterInfo[] memberParameters)
        {
            var methodArgumentTypes = new Type[memberParameters.Length];
            var defaultArgumentValues = new object[memberParameters.Length];

            ParameterInfo memberParameter;
            for (int i = 0; i < memberParameters.Length; i++)
            {
                memberParameter = memberParameters[i];
                methodArgumentTypes[i] = memberParameter.ParameterType;
                if (memberParameter.HasDefaultValue)
                {
                    defaultArgumentValues[i] = memberParameter.DefaultValue;
                }
            }

            return (methodArgumentTypes, defaultArgumentValues);
        }

        private static bool TryResolveArgumentsInternal(Type[] memberArgumentTypes, object[] args,
            ArgumentsResolveRule argumentsResolveRule, out object[] resolvedArgs, object[]? defaultValues = null)
        {
            if (!StraightArgumentsArray(memberArgumentTypes.Length, ref args, argumentsResolveRule))
            {
                resolvedArgs = Array.Empty<object>();
                return false;
            }

            int[] resolvedArgIndexes = GetIndexSpecializedArray(memberArgumentTypes.Length);
            for (int i = 0; i < memberArgumentTypes.Length; i++)
            {
                for (int j = 0; j < args.Length; j++)
                {
                    if (memberArgumentTypes[i].IsInstanceOfType(args[j]))
                    {
                        resolvedArgIndexes[i] = j;
                        break;
                    }
                }

                if (!ValidateResolve(() => resolvedArgIndexes[i] == -1, argumentsResolveRule, memberArgumentTypes[i].Name))
                {
                    break;
                }
            }

            resolvedArgs = MatchArguments(memberArgumentTypes, defaultValues, args, resolvedArgIndexes);
            return true;
        }

        private static int[] GetIndexSpecializedArray(int argumentsCount)
        {
            var resolvedArgIndexes = new int[argumentsCount];
            resolvedArgIndexes.Fill(-1);

            return resolvedArgIndexes;
        }

        private static bool StraightArgumentsArray(int argumentsCount, ref object[] args, ArgumentsResolveRule argumentsResolveRule)
        {
            if (argumentsCount > args.Length)
            {
                switch (argumentsResolveRule)
                {
                    case ArgumentsResolveRule.DefaultWhenMissing:
                        Array.Resize(ref args, argumentsCount);
                        break;

                    case ArgumentsResolveRule.ThrowIfMissing:
                        throw new Exception("All arguments must be setted.");
                    case ArgumentsResolveRule.BreakWhenMissing:
                        return false;

                    default:
                        break;
                }
            }

            return true;
        }

        private static bool ValidateResolve(Func<bool> breakPredicate, ArgumentsResolveRule argumentsResolveRule,
            string argumentTypeName)
        {
            if (!breakPredicate())
            {
                return true;
            }

            switch (argumentsResolveRule)
            {
                case ArgumentsResolveRule.ThrowIfMissing:
                    throw new Exception($"Argument {argumentTypeName} wasn't found.");
                case ArgumentsResolveRule.BreakWhenMissing:
                    return false;

                default:
                    break;
            }

            return true;
        }

        private static object[] MatchArguments(Type[] memberArgumentTypes, object[]? defaultValues, object[] args,
            int[] resolvedArgIndexes)
        {
            if (defaultValues.IsNullOrEmpty())
            {
                defaultValues = new object[memberArgumentTypes.Length];
            }

            object[] resolvedArgs = new object[memberArgumentTypes.Length];
            for (int i = 0; i < resolvedArgIndexes.Length; i++)
            {
                if (resolvedArgIndexes[i] == -1)
                {
                    resolvedArgs[i] = defaultValues[i];
                }
                else
                {
                    resolvedArgs[i] = args[resolvedArgIndexes[i]];
                }
            }

            return resolvedArgs;
        }
    }
}
