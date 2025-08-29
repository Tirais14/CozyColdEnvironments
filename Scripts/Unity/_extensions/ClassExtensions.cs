using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using CozyColdEnvironments.Diagnostics;
using CozyColdEnvironments.Linq;
using CozyColdEnvironments.Reflection;
using Object = UnityEngine.Object;

#nullable enable
namespace CozyColdEnvironments.Unity.Extensions
{
    public static class ClassExtensions
    {
        public static T ThrowIfNotFound<T>(this T? obj)
            where T : Object
        {
            if (obj == null)
            {
                if (typeof(T).IsType<Component>())
                    throw new ComponentNotFoundException(typeof(T));
                else
                    throw new ObjectNotFoundException(typeof(T));
            }

            return obj;
        }

        public static T ThrowIfNull<T, TException>(this T? obj, TException exception)
         where T : class
         where TException : Exception
        {
            if (obj.IsNull())
                throw exception;

            return obj;
        }

        public static T ThrowIfNull<T>(this T? obj, string message)
            where T : class => obj.ThrowIfNull(new NullReferenceException(message));

        public static T ThrowIfNull<T>(this T? obj)
            where T : class => obj.ThrowIfNull(new NullReferenceException());

        public static T IfNull<T>(this T? obj, T value)
            where T : class
        {
            if (obj.IsNull())
                return value;

            return obj;
        }
        public static T IfNull<T>(this T? obj, Func<T> action)
            where T : class
        {
            if (obj.IsNull())
                return action();

            return obj;
        }

        public static void IfNotNull<T>(this T? obj, Action<T> action)
            where T : class
        {
            if (obj.IsNotNull())
                action(obj);
        }

        public static T? IfNotNull<T>(this T? obj, Func<T, T> action, T? ifNull = default!)
            where T : class
        {
            if (obj.IsNotNull())
                return action(obj);

            return ifNull;
        }

        public static T IfNotNull<T>(this T? obj, Func<T, T> action, Func<T> ifNull)
            where T : class
        {
            if (obj.IsNotNull())
                return action(obj);

            return ifNull();
        }

        public static TOut? IfNotNull<TIn, TOut>(this TIn? obj, Func<TIn, TOut> action, TOut? ifNull = default)
            where TIn : class
        {
            if (obj.IsNotNull())
                return action(obj);

            return ifNull;
        }

        public static TOut IfNotNull<TIn, TOut>(this TIn? obj, Func<TIn, TOut> action, Func<TOut> ifNull)
            where TIn : class
        {
            if (obj.IsNotNull())
                return action(obj);

            return ifNull();
        }
    }
}
