#nullable enable
using CCEnvs.Async;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Json;
using CCEnvs.Json.Converters;
using CCEnvs.Reflection;
using CCEnvs.Returnables;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            ExpirationScanFrequency = 5.Seconds(),
        });
        public static AsyncTaskRegistry NeccesaryTasks { get; } = new();
        public static AsyncTaskRegistry BackgroundTasks { get; } = new();
        public static object EmptyObject { get; } = new object();
        public static object[] EmptyArguments { get; } = Array.Empty<object>();
        public static string WordSeparator { get; set; } = "_";
        public static Func<bool> TruePredicate { get; } = static () => true;
        public static Func<bool> FalsePredicate { get; } = static () => false;
        public static JsonSerializerSettings JsonOptions { get; } = JsonSerializerSettingsProvider.GetDefault();
        public static JsonSerializerSettings DebugJsonOptions { get; } = JsonSerializerSettingsProvider.GetDefault().AddConverters(new DebugJsonConverter());
        //public static JsonSerializerOptions JsonOptionsPolymorph { get; } = JsonSerilizerOptionsProvider.GetDefaultPolymorph();

        public static void Install()
        {
        }

#pragma warning disable S112
        public static class Throw
        {
            [DoesNotReturn]
            public static object InvalidCast(Type toType,
                                                string? message = null,
                                                Exception? innerException = null)
            {
                throw new InvalidCastException($"Conversation type = {toType.GetFullName()}. {message}", innerException);
            }

            [DoesNotReturn]
            public static object InvalidCast(Type fromType,
                                                Type toType,
                                                string? message = null,
                                                Exception? innerException = null)
            {
                throw new InvalidCastException($"From {fromType.GetFullName()} to {toType.GetFullName()}. {message}", innerException);
            }

            [DoesNotReturn]
            public static object IndexOutOfRange(long index)
            {
                throw new IndexOutOfRangeException($"Index = {index}.");
            }

            [DoesNotReturn]
            public static object ArgumentException(object argument, string argName)
            {
                throw new ArgumentException($"Argument: {argName} cannot be {argName}.");
            }

            [DoesNotReturn]
            public static object InvalidOperation(object arg, string? argName = null)
            {
                throw new InvalidOperationException($"Argument: {argName ?? "value"} cannot be {arg}");
            }

            [DoesNotReturn]
            public static object MemberNotFound(string? name = null, MemberTypes? memberType = null, Type? reflectedType = null, BindingFlags? bindingFlags = null, Type[]? argumentTypes = null, Binder? binder = null)
            {
                string msg = Sentence.Empty.Add($"name '{name}'", name)
                    .AddIfNotDefault($"member type '{memberType}'", memberType)
                    .AddIfNotDefault($"reflected type '{reflectedType}'", reflectedType)
                    .AddIfNotDefault($"binding flags '{bindingFlags}'", bindingFlags)
                    .AddIfNotDefault($"argument types '{argumentTypes.Select(x => x.ToString()).Aggregate((left, right) => left + right)}'", argumentTypes)
                    .AddIfNotDefault($"binder '{binder}'", binder)
                    .ToString();

                throw new InvalidOperationException($"Member not found. {msg}");
            }

            [DoesNotReturn]
            public static object EndlessLoop(ulong iterationCount, string? msg = null)
            {
                throw new InvalidOperationException($"Prevented endless loop with interation count '{iterationCount}'. {msg}");
            }

            [DoesNotReturn]
            public static object MetedataNotFound(MemberInfo member)
            {
                throw new InvalidOperationException($"Metadata not found. Member info: name '{member}', type '{member.MemberType}'");
            }
        }
#pragma warning restore S112

        public static class Guard
        {
            /// <exception cref="ArgumentNullException"></exception>
            public static void IsNotNull<T>([NotNull] T? obj, string? paramName)
            {
                if (obj.IsNull())
                    throw new ArgumentNullException(paramName ?? "value");
            }

            public static void IsNotNullTarget<T>([NotNull] T? obj)
            {
                if (obj.IsNull())
                    throw new ArgumentNullException("target");
            }

            public static void IsNotNullSource<T>([NotNull] T? obj)
            {
                if (obj.IsNull())
                    throw new ArgumentNullException("source");
            }
        }
    }
}