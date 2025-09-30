#nullable enable

using CCEnvs.Async;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Returnables;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
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
        public static AsyncTaskRegistry NeccesaryTasks { get; } = new();
        public static AsyncTaskRegistry BackgroundTasks { get; } = new();
        public static object EmptyObject { get; } = new object();
        public static object[] EmptyArguments { get; } = Array.Empty<object>();
        public static string WordSeparator { get; set; } = "_";

        public static Result DoNothing() => new();

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
        }
#pragma warning restore S112

        public static class Validate
        {
            /// <exception cref="ArgumentNullException"></exception>
            public static void ArgumentNull<T>([NotNull] T? obj,
                                            string paramName)
            {
                if (obj.IsNull())
                    throw new ArgumentNullException(paramName);
            }
            public static void ArgumentNullNested<T>([NotNull] T? obj,
                                                  params string[] complexParamName)
            {
                if (obj.IsNull())
                    throw new ArgumentNullException(complexParamName.JoinStrings('.'));
            }

            public static void Argument<T>(T value,
                                           string paramName,
                                           bool mustBeTrue,
                                           string? message = null)
            {
                if (!mustBeTrue)
                    throw new ArgumentException($"{paramName} = {value}. {message}");
            }

            public static void ArgumentNested<T>(T value,
                                                 bool mustBeTrue,
                                                 params string[] complexParamName)
            {
                if (!mustBeTrue)
                    throw new ArgumentException($"{complexParamName.JoinStrings('.')} = {value}.");
            }

            public static void StringArgument([NotNull] string? value, string paramName)
            {
                if (value.IsNullOrEmpty())
                    throw new StringArgumentException(paramName, value);
            }

            public static void StringArgumentNested([NotNull] string? value,
                                                    params string[] nameParts)
            {
                if (value.IsNullOrEmpty())
                    throw new StringArgumentException(nameParts.JoinStrings('.'), value);
            }

            public static void String([NotNull] string? value)
            {
                if (value.IsNullOrEmpty())
                    throw new StringException(value);
            }

            /// <exception cref="CollectionArgumentException"></exception>
            public static void CollectionArgument([NotNull] IEnumerable? enumerable,
                                                  string paramName)
            {
                if (CCEnumerable.IsNullOrEmpty(enumerable))
                    throw new CollectionArgumentException(paramName, enumerable);
            }

            public static void Collection([NotNull] IEnumerable? enumerable)
            {
                if (CCEnumerable.IsNullOrEmpty(enumerable))
                    throw new CollectionException(enumerable);
            }
        }
    }
}