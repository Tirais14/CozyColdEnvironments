#nullable enable
using CCEnvs.Async;
using CCEnvs.Json;
using CCEnvs.Json.Converters;
using CCEnvs.Patterns.Commands;
using CCEnvs.Reflection;
using Cysharp.Threading.Tasks;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using R3;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs
{
    public delegate Task<TOutput> ConverterAsync<in TInput, TOutput>(TInput input);
    public delegate void ActionPredicated<T>(Predicate<T> predicate, T value);

    /// <summary>
    /// Must be null after call
    /// </summary>
    public delegate void SingleUseAction();

    public static class CC
    {
        public const string FULL_NAME = "CozyColdEnvironments";
        public const string COPYRIGHT_STAMP = "@Tirais: " + FULL_NAME;

        public static MemoryCache Cache { get; } = new(new MemoryCacheOptions
        {
            ExpirationScanFrequency = 1.Seconds(),
        });
        public static AsyncTaskRegistry NeccesaryTasks { get; } = new();
        public static AsyncTaskRegistry BackgroundTasks { get; } = new();
        public static object EmptyObject { get; } = new object();
        public static object[] EmptyArguments { get; } = Array.Empty<object>();
        public static string WordSeparator { get; set; } = "_";
        public static Func<bool> TruePredicate { get; } = static () => true;
        public static Func<bool> FalsePredicate { get; } = static () => false;
        public static JsonSerializerSettings JsonSettings { get; } = JsonSerializerSettingsProvider.GetDefault();
        public static JsonSerializerSettings DebugJsonSettings { get; } = JsonSerializerSettingsProvider.GetDefault().AddConverters(new DebugJsonConverter());
        public static CommandScheduler CommandScheduler { get; }

        static CC()
        {
            CommandScheduler = new CommandScheduler();
            var frameProvider = ObservableSystem.DefaultFrameProvider ?? new TimerFrameProvider(1.Milliseconds());
            frameProvider.Register(CommandScheduler);
        }

        public static void Install()
        {
        }

#if UNITASK_PLUGIN
        public static UniTask RegisterAsNeccessaryTask(this UniTask source)
        {
            NeccesaryTasks.RegisterTask(source);
            return source;
        }

        public static UniTask<T> RegisterAsNeccessaryTask<T>(this UniTask<T> source)
        {
            NeccesaryTasks.RegisterTask(source);
            return source;
        }
#endif

        public static ValueTask RegisterAsNeccessaryTask(this ValueTask source)
        {
            NeccesaryTasks.RegisterTask(source);
            return source;
        }

        public static ValueTask<T> RegisterAsNeccessaryTask<T>(this ValueTask<T> source)
        {
            NeccesaryTasks.RegisterTask(source);
            return source;
        }

        public static Task RegisterAsNeccessaryTask(this Task source)
        {
            NeccesaryTasks.RegisterTask(source);
            return source;
        }

#if UNITASK_PLUGIN
        public static UniTask RegisterAsBackgroundTask(this UniTask source)
        {
            BackgroundTasks.RegisterTask(source);
            return source;
        }

        public static UniTask<T> RegisterAsBackgroundTask<T>(this UniTask<T> source)
        {
            BackgroundTasks.RegisterTask(source);
            return source;
        }
#endif

        public static ValueTask RegisterAsBackgroundTask(this ValueTask source)
        {
            BackgroundTasks.RegisterTask(source);
            return source;
        }

        public static ValueTask<T> RegisterAsBackgroundTask<T>(this ValueTask<T> source)
        {
            BackgroundTasks.RegisterTask(source);
            return source;
        }

        public static Task RegisterAsBackgroundTask(this Task source)
        {
            BackgroundTasks.RegisterTask(source);
            return source;
        }

#pragma warning disable S112

        public static class ThrowHelper
        {
            public static InvalidCastException InvalidCastException(Type toType,
                          string? message = null,
                          Exception? innerException = null)
            {
                return new InvalidCastException($"Conversation type = {toType.GetFullName()}. {message}", innerException);
            }

            public static InvalidCastException InvalidCastException(Type fromType,
                                                Type toType,
                                                string? message = null,
                                                Exception? innerException = null)
            {
                return new InvalidCastException($"From {fromType.GetFullName()} to {toType.GetFullName()}. {message}", innerException);
            }

            public static IndexOutOfRangeException IndexOutOfRangeException(long index)
            {
                return new IndexOutOfRangeException($"Index = {index}.");
            }

            public static ArgumentException ArgumentExceptionException(object argument, string argName)
            {
                return new ArgumentException($"Argument: {argName} cannot be {argName}.");
            }

            public static InvalidOperationException InvalidOperationException(object arg, string? argName = null)
            {
                return new InvalidOperationException($"Argument: {argName ?? "value"} cannot be {arg}");
            }

            public static InvalidOperationException MemberNotFoundException(string? name = null, MemberTypes? memberType = null, Type? reflectedType = null, BindingFlags? bindingFlags = null, Type[]? argumentTypes = null, Binder? binder = null)
            {
                string msg = Sentence.Empty.AddIfNotDefault($"name \"{name}\"", name)
                    .AddIfNotDefault($"member type \"{memberType}\"", memberType)
                    .AddIfNotDefault($"reflected type \"{reflectedType}\"", reflectedType)
                    .AddIfNotDefault($"binding flags \"{bindingFlags}\"", bindingFlags)
                    .AddIfNotDefault($"argument types \"{argumentTypes?.Select(x => x.ToString()).Aggregate((left, right) => left + right)}\"", argumentTypes)
                    .AddIfNotDefault($"binder \"{binder}\"", binder)
                    .ToString();

                return new InvalidOperationException($"Member not found. {msg}");
            }

            public static InvalidOperationException EndlessLoopException(ulong iterationCount, string? msg = null)
            {
                return new InvalidOperationException($"Prevented endless loop with interation count \"{iterationCount}\". {msg}");
            }

            public static InvalidOperationException MetadataNotFound(MemberInfo member)
            {
                Guard.IsNotNull(member, nameof(member));

                return new InvalidOperationException($"Metadata not found. Member info: name '{member}', type \"{member.MemberType}\"");
            }

            public static InvalidOperationException TypeNotFoundException(string? typeName, string? assemblyName = null)
            {
                return new InvalidOperationException($"Type name '{typeName}', assembly name \"{assemblyName}\"");
            }

            public static ArgumentException IsNotTypeException(Type? left, Type? right, string? paramName = null)
            {
                return new ArgumentException($"Invalid argument '{paramName ?? "value"}'. Left type '{(left?.ToString() ?? "null")}' is not \"{(right?.ToString() ?? "null")}\"");
            }
        }


#pragma warning restore S112

        public static class Guard
        {
            /// <exception cref="ArgumentNullException"></exception>
            public static void IsNotNull<T>([NotNull] T? obj, string? paramName = null)
            {
                if (obj.IsNull())
                    throw new ArgumentNullException(paramName ?? "value");
            }

            public static void IsNotNullTarget<T>([NotNull] T? obj)
            {
                IsNotNull(obj, "target");
            }

            public static void IsNotNullSource<T>([NotNull] T? obj)
            {
                IsNotNull(obj, "source");
            }

            public static void IsNotNullState<T>([NotNull] T? obj)
            {
                IsNotNull(obj, "state");
            }

            public static void IsNotNullInput<T>([NotNull] T? obj)
            {
                IsNotNull(obj, "input");
            }

            public static void IsNotType(Type left, Type right, string? paramName = null)
            {
                CommunityToolkit.Diagnostics.Guard.IsNotNull(left, nameof(left));
                CommunityToolkit.Diagnostics.Guard.IsNotNull(right, nameof(right));

                if (left.IsNotType(right))
                    throw ThrowHelper.IsNotTypeException(left, right, paramName);
            }

            public static void IsNotType<TRight>(Type left, string? paramName = null)
            {
                IsNotType(left, typeof(TRight), paramName);
            }

            public static void IsNotDefault<T>(T value, string paramName = "value")
                where T : struct
            {
                if (value.IsDefault())
                    throw new ArgumentException($"{paramName} is default");
            }
        }
    }
}