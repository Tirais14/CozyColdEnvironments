using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UTIRLib.Attributes;
using UTIRLib.Utils;

#nullable enable
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one

namespace UTIRLib.Initializer
{
    [Obsolete]
    public static class InitializeHelper
    {
        public static void InitializeObject(object target, InitializeParameters parameters, params object?[] args) =>
            InitializeObjectInternal(target, (Delegate)null!, parameters, args);

        public static void InitializeObject(object target, params object?[] args) =>
            InitializeObjectInternal(target, (Delegate)null!, InitializeParameters.ArgumentsNotNull, args);

        public static void InitializeObject<TMethod>(object target, TMethod method, InitializeParameters parameters,
            params object?[] args)
            where TMethod : Delegate => InitializeObjectInternal(target, method, parameters, args);

        public static void InitializeObject<TMethod>(object target, TMethod method, params object?[] args)
            where TMethod : Delegate => InitializeObjectInternal(target, method, InitializeParameters.ArgumentsNotNull, args);

        public static void InitializeObject(object target, string methodName, InitializeParameters parameters, params object?[] args) =>
            InitializeObjectInternal(target, target.GetType().GetMethod(methodName), parameters, args);

        public static void InitializeObject(object target, string methodName, params object?[] args) =>
            InitializeObjectInternal(target, target.GetType().GetMethod(methodName), InitializeParameters.ArgumentsNotNull, args);

        //public static void InitializeObject<T>(object target, Action<T> method, T arg,
        //    InitializeParameters parameters = InitializeParameters.ArgumentsNotNull) =>
        //    InitializeObjectInternal(target, method, InitializeParameters.ArgumentsNotNull, arg, parameters);
        //public static void InitializeObject<T1, T2>(object target, Action<T1, T2> method, T1 arg1, T2 arg2,
        //    InitializeParameters parameters = InitializeParameters.ArgumentsNotNull) =>
        //    InitializeObjectInternal(target, method, InitializeParameters.ArgumentsNotNull, arg1, arg2, parameters);
        //public static void InitializeObject<T1, T2, T3>(object target, Action<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3,
        //    InitializeParameters parameters = InitializeParameters.ArgumentsNotNull) =>
        //    InitializeObjectInternal(target, method, InitializeParameters.ArgumentsNotNull, arg1, arg2, arg3, parameters);
        //public static void InitializeObject<T1, T2, T3, T4>(object target, Action<T1, T2, T3, T4> method, T1 arg1, T2 arg2,
        //    T3 arg3, T4 arg4, InitializeParameters parameters = InitializeParameters.ArgumentsNotNull) =>
        //    InitializeObjectInternal(target, method, InitializeParameters.ArgumentsNotNull, arg1, arg2, arg3, arg4, parameters);
        //public static void InitializeObject<T1, T2, T3, T4, T5>(object target, Action<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2,
        //    T3 arg3, T4 arg4, T5 arg5, InitializeParameters parameters = InitializeParameters.ArgumentsNotNull) =>
        //    InitializeObjectInternal(target, method, InitializeParameters.ArgumentsNotNull, arg1, arg2, arg3, arg4, arg5, parameters);
        //public static void InitializeObject<T1, T2, T3, T4, T5, T6>(object target, Action<T1, T2, T3, T4, T5, T6> method, T1 arg1,
        //    T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, InitializeParameters parameters = InitializeParameters.ArgumentsNotNull) =>
        //    InitializeObjectInternal(target, method, InitializeParameters.ArgumentsNotNull, arg1, arg2, arg3, arg4, arg5, arg6, parameters);
        //public static void InitializeObject<T1, T2, T3, T4, T5, T6, T7>(object target, Action<T1, T2, T3, T4, T5, T6, T7> method,
        //    T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7,
        //    InitializeParameters parameters = InitializeParameters.ArgumentsNotNull) =>
        //    InitializeObjectInternal(target, method, InitializeParameters.ArgumentsNotNull, arg1, arg2, arg3, arg4, arg5, arg6, arg7, parameters);

        public static void InvokeMethods(InitializeMethodInfo[] initializeMethods, params object?[] args)
        {
            foreach (var initializeMethod in initializeMethods)
            {
                initializeMethod.Invoke(args);
            }
        }

        public static InitializeMethodInfo[] GetInitializeMethods(object source) =>
            SelectInitializeMethods(source, TypeHelper.GetAllMembers<MethodInfo>(source.GetType(),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

        private static PropertyInfo GetIsInitializedProperty(Type type) => type.GetProperty(nameof(IInitializableBase.IsInitialized)) ??
                throw new InvalidOperationException($"{nameof(IInitializableBase.IsInitialized)} property not found in {type.Name}.");

        private static void InitializeObjectInternal(object obj, Delegate? method, InitializeParameters? parameters,
            params object?[] args)
        {
            if (args.Length > 0 && parameters.HasValue && parameters.Value.HasFlag(InitializeParameters.ArgumentsNotNull))
            {
                CheckArguments(args);
            }

            PropertyInfo isInitializedProp = GetIsInitializedProperty(obj.GetType());
            if ((bool)isInitializedProp.GetValue(obj))
            {
                //Debug.LogWarning(new AlreadyInitializedMessage(obj));
                return;
            }

            if (method is not null)
            {
                method.DynamicInvoke(args);
            }
            else
            {
                InitializeMethodInfo[] methods = GetInitializeMethods(obj);
                InvokeMethods(methods, args);
            }

            isInitializedProp?.SetValue(obj, true);
        }

        private static void InitializeObjectInternal(object obj, MethodInfo method, InitializeParameters? parameters,
            params object?[] args)
        {
            if (args.Length > 0 && parameters.HasValue && parameters.Value.HasFlag(InitializeParameters.ArgumentsNotNull))
            {
                CheckArguments(args);
            }

            PropertyInfo isInitializedProp = GetIsInitializedProperty(obj.GetType());
            if ((bool)isInitializedProp.GetValue(obj))
            {
                //Debug.LogWarning(new AlreadyInitializedMessage(obj));
                return;
            }

            method.Invoke(obj, args);
            isInitializedProp?.SetValue(obj, true);
        }

        private static InitializeMethodInfo[] SelectInitializeMethods(object source, MethodInfo[] methods)
        {
            List<InitializeMethodInfo> selectedMethods = new(methods.Length / 2);
            InitAttribute attribute;
            for (int i = 0; i < methods.Length; i++)
            {
                attribute = methods[i].GetCustomAttribute<InitAttribute>();
                if (attribute != null)
                {
                    selectedMethods.Add(new InitializeMethodInfo(source, methods[i], attribute));
                }
            }

            return selectedMethods.OrderBy((value) => value.attribute.Order).ToArray();
        }

        private static void CheckArguments(object?[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                {
                    throw new ArgumentNullException();
                }
            }
        }
    }
}