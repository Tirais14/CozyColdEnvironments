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

        public static object Create(this IFactory factory, params object[]? args)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));
            args ??= Array.Empty<object>();

            var argTypesHashBuilder = new HashCode();
            argTypesHashBuilder.Add(factory.GetType());

            for (int i = 0; i < args.Length; i++)
                argTypesHashBuilder.Add(args[i].GetType());

            int argTypesHash = argTypesHashBuilder.ToHashCode();

            if (!methodCache.TryGetValue<MethodInfo>(argTypesHash, out var method) ||
                method is null)
            {
                var factoryType = factory.GetType();
                var methods = factoryType.GetMethods(BindingFlagsDefault.InstancePublic);

                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo m = methods[i];

                    if (m.Name != "Create")
                        continue;

                    var parameters = m.GetParameters();

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

                    if (!isTarget)
                        continue;

                    method = m;
                    using var cacheEntry = methodCache.CreateEntry(argTypesHash);
                    cacheEntry.Value = method;
                    cacheEntry.AbsoluteExpirationRelativeToNow = 6.Minutes();
                    break;
                }

                if (method is null)
                {
                    throw new InvalidOperationException(DebugMessageBuilder.CreatePooled()
                        .AddMessage("Cannot resolve factory method")
                        .AddProperty("Factory", factory.GetType())
                        .AddProperty("Args", args.Select(x => x.GetType()).SequenceToString())
                        .AddProperty("FoundMethods", methods.SequenceToString())
                        .ToStringAndDispose()
                        );
                }
            }

            CCDebug.PrintLog($"Method: {method}; Args: {args.Select(x => x.GetType()).SequenceToString()}");

            return method.Invoke(factory, args);
        }

        [OnInstallExecutable]
        private static void OnInstall()
        {
            methodCache.Clear();
        }
    }
}