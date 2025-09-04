#nullable enable
using CCEnvs.Collections;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Diagnostics
{
    public static class ThrowHelper
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

        public static void Argument([NotNull] object? value,
                                    string paramName,
                                    bool condition)
        {
            if (!condition || value.IsNull())
                throw new ArgumentException($"{paramName} = {value?.ToString() ?? "null"}");
        }

        public static void ArgumentNested(object? value,
                                          bool condition,
                                          params string[] complexParamName)
        {
            if (!condition || value.IsNull())
                throw new ArgumentException($"{complexParamName.JoinStrings('.')} = {value?.ToString() ?? "null"}");
        }

        public static void StringArgument(string paramName, [NotNull] string? value)
        {
            if (value.IsNullOrEmpty())
                throw new StringArgumentException(paramName, value);
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
