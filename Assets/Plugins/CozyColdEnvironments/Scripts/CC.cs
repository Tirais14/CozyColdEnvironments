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
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
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
            public static ThrowVoid InvalidCast(Type toType,
                                                string? message = null,
                                                Exception? innerException = null)
            {
                throw new InvalidCastException($"Conversation type = {toType.GetFullName()}. {message}", innerException);
            }

            [DoesNotReturn]
            public static ThrowVoid InvalidCast(Type fromType,
                                                Type toType,
                                                string? message = null,
                                                Exception? innerException = null)
            {
                throw new InvalidCastException($"From {fromType.GetFullName()} to {toType.GetFullName()}. {message}", innerException);
            }

            [DoesNotReturn]
            public static ThrowVoid IndexOutOfRange(long index)
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
        }
#pragma warning restore S112

        public static class Guard
        {
            public static void SourceArg<T>([NotNull] T? obj)
            {
                IsNotNull(obj, "source");
            }

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

            /// <exception cref="ArgumentException"></exception>
            public static void Argument(bool mustBeFalse,
                                        string paramName,
                                        string? message = null)
            {
                if (mustBeFalse)
                    throw new ArgumentException(message, paramName);
            }
            public static void Argument<T>(bool mustBeFalse,
                                           T param,
                                           string paramName,
                                           string? message = null)
            {
                if (mustBeFalse)
                    throw new ArgumentException($"Param: {param}; {message}", paramName);
            }

            [Obsolete("Use Argument instead.")]
            /// <exception cref="ArgumentException"></exception>
            public static void ArgumentObsolete<T>(T value,
                                                   string paramName,
                                                   bool mustBeTrue,
                                                   string? message = null)
            {
                if (!mustBeTrue)
                    throw new ArgumentException(message, paramName);
            }

            /// <exception cref="EmptyStringArgumentException"></exception>
            public static void StringArgument([NotNull] string? value, string paramName)
            {
                IsNotNull(value, paramName);

                if (value == string.Empty)
                    throw new EmptyStringArgumentException(paramName, value);
            }

            /// <exception cref="EmptyStringException"></exception>
            public static void String(string value)
            {
                if (value.IsNullOrEmpty())
                    throw new EmptyStringException(value);
            }

            /// <exception cref="EmptyCollectionArgumentException"></exception>
            public static void CollectionArgument([NotNull] IEnumerable? enumerable,
                                                  string paramName)
            {
                IsNotNull(enumerable, paramName);

                if (CCEnumerable.IsNullOrEmpty(enumerable))
                    throw new EmptyCollectionArgumentException(paramName);
            }

            /// <exception cref="EmptyCollectionException"></exception>
            public static void Collection(IEnumerable enumerable)
            {
                if (CCEnumerable.IsEmpty(enumerable))
                    throw new EmptyCollectionException();
            }
        }
    }
}