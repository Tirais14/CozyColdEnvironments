#pragma warning disable S2436
using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Reflection;

namespace CCEnvs.Patterns.Factories
{
#nullable enable

    public interface IFactory
    {
        //object Create(object[] args);
    }

    public interface IFactory<out TOut> : IFactory
    {
        TOut Create();

        //object IFactory.Create(object[] args) => Create()!;
    }

    public interface IFactory<in T, out TOut> : IFactory
    {
        TOut Create(T arg);

        //object IFactory.Create(object[] args) => Create(args[0].CastTo<T>())!;
    }

    public interface IFactory<in T, in T1, out TOut> : IFactory
    {
        TOut Create(T arg, T1 arg1);

        //object IFactory.Create(object[] args)
        //{
        //    return Create(args[0].CastTo<T>(), args[1].CastTo<T1>())!;
        //}
    }

    public interface IFactory<in T, in T1, in T2, out TOut> : IFactory
    {
        TOut Create(T arg, T1 arg1, T2 arg2);

        //object IFactory.Create(object[] args)
        //{
        //    return Create(args[0].CastTo<T>(), args[1].CastTo<T1>(), args[2].CastTo<T2>())!;
        //}
    }

    public static class IFactoryExtensions
    {
        private static readonly MemoryCache methodCache = new(
            new MemoryCacheOptions
            {
                ExpirationScanFrequency = 2.Minutes()
            });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="uniqueID">maybe null only if Factory realizes <see cref="IFactoryNonGeneric"/></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static object Create(this IFactory factory, object uniqueID, object[]? args)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));
            CC.Guard.IsNotNull(uniqueID, nameof(uniqueID));
            args ??= Array.Empty<object>();

            if (factory is IFactoryNonGeneric nonGenericFactory)
                return nonGenericFactory.Create(args);

            var argTypesHashBuilder = new HashCode();
            argTypesHashBuilder.Add(factory.GetType());

            for (int i = 0; i < args.Length; i++)
                argTypesHashBuilder.Add(args[i].GetType());

            int argTypesHash = argTypesHashBuilder.ToHashCode();
            var cacheID = (uniqueID, argTypesHash);

            if (!methodCache.TryGetValue<MethodInfo>(cacheID, out var method) ||
                method is null)
            {
                method = ResolveFactoryCreateMethod(factory, args);
                using var cacheEntry = methodCache.CreateEntry(cacheID);
                cacheEntry.Value = method;
                cacheEntry.SlidingExpiration = 6.Minutes();
            }

            return method.Invoke(factory, args);
        }

        public static object Create(this IFactory factory, object[]? args)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));
            args ??= Array.Empty<object>();

            return ResolveFactoryCreateMethod(factory, args).Invoke(factory, args);
        }

        private static MethodInfo ResolveFactoryCreateMethod(
            IFactory factory,
            object[] args
            )
        {
            var factoryType = factory.GetType();
            var methods = factoryType.GetMethods(BindingFlagsDefault.InstancePublic);

            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];

                if (method.Name != "Create")
                    continue;

                var parameters = method.GetParameters();

                if (parameters.Length > args.Length)
                    continue;

                bool isTarget = true;

                for (int j = 0; j < parameters.Length; j++)
                {
                    ParameterInfo param = parameters[i];

                    if (args[j].IsNotInstanceOfType(param.ParameterType))
                    {
                        isTarget = false;
                        break;
                    }
                }

                if (isTarget)
                    return method;
            }

            throw new InvalidOperationException(DebugMessageBuilder.CreatePooled()
                .AddMessage("Cannot resolve factory method")
                .AddProperty("Factory", factory.GetType())
                .AddProperty("Args", args.Select(x => x.GetType()).SequenceToString())
                .AddProperty("FoundMethods", methods.SequenceToString())
                .ToStringAndDispose()
                );
        } 

        [OnInstallExecutable]
        private static void OnInstall()
        {
            methodCache.Clear();
        }
    }
}