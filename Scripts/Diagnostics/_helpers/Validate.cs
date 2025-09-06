#nullable enable
using CCEnvs.Collections;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Diagnostics
{
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

        public static void CollectionArgument(string paramName, [NotNull] IEnumerable? enumerable)
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
