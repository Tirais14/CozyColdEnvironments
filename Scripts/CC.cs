#nullable enable

using CCEnvs.Async;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Returnables;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs
{
    public static class CC
    {
        public static AsyncTaskRegistry NeccesaryTasks { get; } = new();
        public static AsyncTaskRegistry BackgroundTasks { get; } = new();
        public static object EmptyObject { get; } = new object();
        public static object[] EmptyArguments { get; } = Array.Empty<object>();
        public static string WordSeparator { get; set; } = "_";

        public static class Create 
        {
            public static T[] Array<T>(params T[] values) => values;
        }

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

        public static class Validate
        {
            public static void ArgumentNull([NotNull] object? obj,
                                            string paramName)
            {
                if (obj.IsNull())
                    throw new ArgumentNullException(paramName);
            }
            public static void ArgumentNullNested([NotNull] object? obj,
                                                  params string[] complexParamName)
            {
                if (obj.IsNull())
                    throw new ArgumentNullException(complexParamName.JoinStrings('.'));
            }

            public static void Argument<T>(T value,
                                           string paramName,
                                           Predicate<T> predicate)
            {
                if (!predicate(value))
                    throw new ArgumentException($"{paramName} = {value}.");
            }

            public static void ArgumentNested<T>(T value,
                                                 Predicate<T> predicate,
                                                 params string[] complexParamName)
            {
                if (!predicate(value))
                    throw new ArgumentException($"{complexParamName.JoinStrings('.')} = {value}.");
            }

            public static void StringArgument(string paramName, [NotNull] string? value)
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

            public static void CollectionArgument(string paramName,
                                                  [NotNull] IEnumerable? enumerable)
            {
                if (CCEnumerable.IsNullOrEmpty(enumerable))
                    throw new CollectionArgumentException(nameof(paramName), enumerable);
            }

            public static void Collection([NotNull] IEnumerable? enumerable)
            {
                if (CCEnumerable.IsNullOrEmpty(enumerable))
                    throw new CollectionException(enumerable);
            }
        }
    }
}