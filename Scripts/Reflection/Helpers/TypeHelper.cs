using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib.Reflection
{
    public static class TypeHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static object?[] GetFieldValues(object target,
            BindingFlags bindingFlags = BindingFlagsDefault.InstanceAll)
        {
            if (target.IsNull())
                throw new ArgumentNullException(nameof(target));

            return target.GetType()
                         .ForceGetFields(bindingFlags)
                         .Select(x => x.GetValue(target))
                         .ToArray();
        }

        /// <exception cref="TypeNotFoundException"></exception>
        public static Type GetPirmitiveType(string shortName, bool throwOnError = true)
        {
            switch (shortName)
            {
                case "byte":
                    return typeof(byte);
                case "sbyte":
                    return typeof(sbyte);
                case "short":
                    return typeof(short);
                case "ushort":
                    return typeof(ushort);
                case "int":
                    return typeof(int);
                case "uint":
                    return typeof(uint);
                case "long":
                    return typeof(long);
                case "ulong":
                    return typeof(ulong);
                case "string":
                    return typeof(string);
                case "bool":
                    return typeof(bool);
                case "object":
                    return typeof(object);
                default:
                    {
                        if (throwOnError)
                            throw new TypeNotFoundException(shortName, "Type hasn't special short name.");
                        return null!;
                    }
            }
        }

        public static bool IsPrimitiveType(Type? type)
        {
            if (type == null) return false;

            return type.IsPrimitive || type.IsAnyType(typeof(decimal), typeof(string));
        }

        public static MemberInfo[] ForceGetMembers(Type type,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var toProccess = new List<Type>{ type };

            var predicate = new LoopPredicate(() => true);
            while (predicate.Invoke())
            {
                type = type.BaseType;

                if (type == typeof(object) || type == null)
                    break;

                toProccess.Add(type);
            }

            bindingFlags |= BindingFlags.DeclaredOnly;
            var members = new List<MemberInfo>();
            int toProccessCount = toProccess.Count;
            for (int i = 0; i < toProccessCount; i++)
                members.AddRange(toProccess[i].GetMembers(bindingFlags));

            return members.ToArray();
        }

        public static T[] ForceGetMembers<T>(Type type,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            return ForceGetMembers(type, bindingFlags).Where(x => x is T)
                                                      .Cast<T>()
                                                      .ToArray();
        }

        /// <summary>
        /// Gets all member from specified type and base types
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public unsafe static T[] GetAllMembers<T>(Type type, BindingFlags bindingFlags = BindingFlags.Default)
            where T : MemberInfo
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            MemberInfo[] members = type.GetMembers(bindingFlags);
            List<T> results = new();

            int membersCount = members.Length;
            for (int i = 0; i < membersCount; i++)
            {
                if (members[i] is T typed)
                    results.Add(typed);
            }

            return results.ToArray();
        }
    }
}

namespace UTIRLib.Reflection.Special
{
    public static class TypeHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static object?[] GetFieldValuesByTypeAndFieldValues(
            object target,
            BindingFlags bindingFlags = BindingFlagsDefault.InstanceAll)
        {
            object?[] targetFieldValues = Reflection.TypeHelper.GetFieldValues(target, bindingFlags);

            if (targetFieldValues.IsEmpty())
                return Array.Empty<object>();

            var results = new List<object?>();
            results.AddRange(targetFieldValues);

            Queue<object?> collected;

            int targetFieldCount = targetFieldValues.Length;
            for (int i = 0; i < targetFieldCount; i++)
            {
                collected = LoopHelper.Collect(targetFieldValues[i], (current) =>
                {
                    if (current is null)
                        return LoopIteration<object?[]>.Void();

                    object?[] fieldValues = Reflection.TypeHelper.GetFieldValues(current, bindingFlags);

                    if (fieldValues.IsNullOrEmpty())
                        return LoopIteration.Continue(fieldValues);
                    else
                        return LoopIteration.Complete(fieldValues);
                });

                results.AddRange(collected);
            }

            return results.ToArray();
        }

        public static FieldInfo[] GetFieldsByTypeAndNestedFieldTypes(Type type,
            BindingFlags bindingFlags = BindingFlagsDefault.InstanceAll)
        {
            FieldInfo[] objFields = type.ForceGetFields(bindingFlags);

            if (objFields.IsEmpty())
                return Array.Empty<FieldInfo>();

            int objFieldCount = objFields.Length;
            var results = new List<FieldInfo>();
            Queue<FieldInfo> collected;

            for (int i = 0; i < objFieldCount; i++)
            {
                collected = LoopHelper.Collect(objFields[i], (current) =>
                {
                    FieldInfo[] tempFields = current.FieldType.ForceGetFields(bindingFlags);

                    if (tempFields.IsEmpty())
                        return LoopIteration<FieldInfo[]>.Void();

                    if (tempFields.IsNullOrEmpty())
                        return LoopIteration.Continue(tempFields);
                    else
                        return LoopIteration.Complete(tempFields);
                });

                results.AddRange(collected);
            }

            return results.ToArray();
        }
    }
}