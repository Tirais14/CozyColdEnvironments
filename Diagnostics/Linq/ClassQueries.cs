using System;

#nullable enable

namespace UTIRLib.Diagnostics
{
    public static class ClassQueries
    {
        public static TObj ThrowIfNull<TObj, TException>(this TObj? obj, TException exception)
         where TObj : class
         where TException : Exception
        {
            if (obj.IsNull())
            {
                throw exception;
            }

            return obj;
        }

        public static TObj ThrowIfNull<TObj>(this TObj? obj, string message)
            where TObj : class => obj.ThrowIfNull(new NullReferenceException(message));

        public static TObj ThrowIfNull<TObj>(this TObj? obj)
            where TObj : class => obj.ThrowIfNull(new NullReferenceException());

        #region Queries

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static object IfNullQ<TObj, TOut>(this TObj? obj, Func<TOut> func)
            where TObj : class => obj.IsNull() ? func : obj;

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNotNullQ<TObj, TOut>(this TObj? obj, Func<TObj, TOut?> func)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                return func(obj);
            }
            else if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return default;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut IfNotNullQ<TObj, TOut>(this TObj? obj, Func<TObj, TOut?> func, TOut elseValue)
            where TObj : class
        {
            TOut? result = obj.IfNotNullQ(func);
            return result.IsNull() ? elseValue : result;
        }

        #endregion Queries

        #region If Null

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNull<TObj>(this TObj? obj, Action action)
            where TObj : class
        {
            if (obj.IsNull())
            {
                action();
            }
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNull<TObj, T1>(this TObj? obj, Action<T1?> action, T1? arg1)
            where TObj : class
        {
            if (obj.IsNull())
            {
                action(arg1);
            }
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNull<TObj, T1, T2>(this TObj? obj, Action<T1?, T2?> action, T1? arg1, T2? arg2)
            where TObj : class
        {
            if (obj.IsNull())
            {
                action(arg1, arg2);
            }
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNull<TObj, T1, T2, T3>(this TObj? obj, Action<T1?, T2?, T3?> action, T1? arg1,
            T2? arg2, T3? arg3)
            where TObj : class
        {
            if (obj.IsNull())
            {
                action(arg1, arg2, arg3);
            }
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNull<TObj, T1, T2, T3, T4>(this TObj? obj, Action<T1?, T2?, T3?, T4?> action, T1? arg1,
            T2? arg2, T3? arg3, T4? arg4)
            where TObj : class
        {
            if (obj.IsNull())
            {
                action(arg1, arg2, arg3, arg4);
            }
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNull<TObj, T1, T2, T3, T4, T5>(this TObj? obj, Action<T1?, T2?, T3?, T4?, T5?> action,
            T1? arg1, T2? arg2, T3? arg3, T4? arg4, T5? arg5)
            where TObj : class
        {
            if (obj.IsNull())
            {
                action(arg1, arg2, arg3, arg4, arg5);
            }
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNull<TObj, T1, T2, T3, T4, T5, T6>(this TObj? obj,
            Action<T1?, T2?, T3?, T4?, T5?, T6?> action, T1? arg1, T2? arg2, T3? arg3, T4? arg4, T5? arg5, T6? arg6)
            where TObj : class
        {
            if (obj.IsNull())
            {
                action(arg1, arg2, arg3, arg4, arg5, arg6);
            }
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNull<TObj, T1, T2, T3, T4, T5, T6, T7>(this TObj? obj,
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?> action, T1? arg1, T2? arg2, T3? arg3, T4? arg4, T5? arg5, T6? arg6,
            T7 arg7)
            where TObj : class
        {
            if (obj.IsNull())
            {
                action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNull<TObj, TOut>(this TObj? obj, Func<TOut?> func, TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNull())
            {
                return func();
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNull<TObj, T1, TOut>(this TObj? obj, Func<T1?, TOut?> func, T1? arg1,
            TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNull())
            {
                return func(arg1);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNull<TObj, T1, T2, TOut>(this TObj? obj, Func<T1?, T2?, TOut?> func, T1? arg1, T2? arg2,
            TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNull())
            {
                return func(arg1, arg2);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNull<TObj, T1, T2, T3, TOut>(this TObj obj, Func<T1?, T2?, T3?, TOut?> func, T1? arg1,
            T2? arg2, T3? arg3, TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNull())
            {
                return func(arg1, arg2, arg3);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNull<TObj, T1, T2, T3, T4, TOut>(this TObj? obj, Func<T1?, T2?, T3?, T4?, TOut?> func,
            T1? arg1, T2? arg2, T3? arg3, T4? arg4, TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNull())
            {
                return func(arg1, arg2, arg3, arg4);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNull<TObj, T1, T2, T3, T4, T5, TOut>(this TObj? obj,
            Func<T1?, T2?, T3?, T4?, T5?, TOut?> func, T1? arg1, T2? arg2, T3? arg3, T4? arg4, T5? arg5,
            TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNull())
            {
                return func(arg1, arg2, arg3, arg4, arg5);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNull<TObj, T1, T2, T3, T4, T5, T6, TOut>(this TObj? obj,
            Func<T1?, T2?, T3?, T4?, T5?, T6?, TOut?> func, T1? arg1, T2? arg2, T3? arg3, T4? arg4, T5? arg5, T6? arg6,
            TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNull())
            {
                return func(arg1, arg2, arg3, arg4, arg5, arg6);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNull<TObj, T1, T2, T3, T4, T5, T6, T7, TOut>(this TObj? obj,
            Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, TOut?> func, T1? arg1, T2? arg2, T3? arg3, T4? arg4, T5? arg5, T6? arg6,
            T7? arg7, TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNull())
            {
                return func(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        #endregion If Null

        #region If Not Null

        /// <summary>If unity or system not <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNotNull<TObj>(this TObj? obj, Action action)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                action();
            }
        }

        /// <summary>If unity or system not <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNotNull<TObj>(this TObj? obj, Action<TObj> action)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                action(obj);
            }
        }

        /// <summary>If unity or system not <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNotNull<TObj, T1>(this TObj? obj, Action<T1?> action, T1? arg1)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                action(arg1);
            }
        }

        /// <summary>If unity or system not <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNotNull<TObj, T1, T2>(this TObj? obj, Action<T1?, T2?> action, T1? arg1, T2? arg2)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                action(arg1, arg2);
            }
        }

        /// <summary>If unity or system not <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNotNull<TObj, T1, T2, T3>(this TObj? obj, Action<T1?, T2?, T3?> action, T1? arg1,
            T2? arg2, T3? arg3)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                action(arg1, arg2, arg3);
            }
        }

        /// <summary>If unity or system not <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNotNull<TObj, T1, T2, T3, T4>(this TObj? obj, Action<T1?, T2?, T3?, T4?> action,
            T1? arg1, T2? arg2, T3? arg3, T4? arg4)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                action(arg1, arg2, arg3, arg4);
            }
        }

        /// <summary>If unity or system not <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNotNull<TObj, T1, T2, T3, T4, T5>(this TObj? obj, Action<T1?, T2?, T3?, T4?, T5?> action,
            T1? arg1, T2? arg2, T3? arg3, T4? arg4, T5? arg5)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                action(arg1, arg2, arg3, arg4, arg5);
            }
        }

        /// <summary>If unity or system not <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNotNull<TObj, T1, T2, T3, T4, T5, T6>(this TObj? obj,
            Action<T1?, T2?, T3?, T4?, T5?, T6?> action, T1? arg1, T2? arg2, T3? arg3, T4? arg4, T5? arg5, T6? arg6)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                action(arg1, arg2, arg3, arg4, arg5, arg6);
            }
        }

        /// <summary>If unity or system not <see langword="null"/> do <see cref="Action"/></summary>
        public static void IfNotNull<TObj, T1, T2, T3, T4, T5, T6, T7>(this TObj? obj,
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?> action, T1? arg1, T2? arg2, T3? arg3, T4? arg4, T5? arg5, T6? arg6,
            T7? arg7)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNotNull<TObj, TOut>(this TObj? obj, Func<TOut?> func, TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                return func();
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNotNull<TObj, T1, TOut>(this TObj? obj, Func<T1?, TOut?> func, T1? arg1,
            TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                return func(arg1);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNotNull<TObj, T1, T2, TOut>(this TObj? obj, Func<T1?, T2?, TOut?> func, T1? arg1,
            T2? arg2, TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                return func(arg1, arg2);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNotNull<TObj, T1, T2, T3, TOut>(this TObj? obj, Func<T1?, T2?, T3?, TOut?> func, T1? arg1,
            T2? arg2, T3? arg3, TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                return func(arg1, arg2, arg3);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNotNull<TObj, T1, T2, T3, T4, TOut>(this TObj? obj, Func<T1?, T2?, T3?, T4?, TOut?> func,
            T1? arg1, T2? arg2, T3? arg3, T4? arg4, TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                return func(arg1, arg2, arg3, arg4);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNotNull<TObj, T1, T2, T3, T4, T5, TOut>(this TObj? obj,
            Func<T1?, T2?, T3?, T4?, T5?, TOut?> func, T1? arg1, T2? arg2, T3? arg3, T4? arg4, T5? arg5,
            TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                return func(arg1, arg2, arg3, arg4, arg5);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNotNull<TObj, T1, T2, T3, T4, T5, T6, TOut>(this TObj? obj,
            Func<T1?, T2?, T3?, T4?, T5?, T6?, TOut?> func, T1? arg1, T2? arg2, T3? arg3, T4? arg4, T5? arg5, T6? arg6,
            TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                return func(arg1, arg2, arg3, arg4, arg5, arg6);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        /// <summary>If unity or system <see langword="null"/> do <see cref="Action"/></summary>
        public static TOut? IfNotNull<TObj, T1, T2, T3, T4, T5, T6, T7, TOut>(this TObj? obj,
            Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, TOut?> func, T1? arg1, T2? arg2, T3? arg3, T4? arg4, T5? arg5, T6? arg6,
            T7? arg7, TOut? defaultValue = default)
            where TObj : class
        {
            if (obj.IsNotNull())
            {
                return func(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }

            if (obj is TOut objTyped)
            {
                return objTyped;
            }
            else return defaultValue;
        }

        #endregion If Not Null
    }
}