using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.TypeMatching
{
    public static class TypeMatch
    {
        public static bool Is<T>(this object? obj)
        {
            if (obj is T && obj.IsNotNull())
                return true;

            return false;
        }
        public static bool Is<TThis, T>(this TThis? obj)
        {
            if (obj is T && obj.IsNotNull())
                return true;

            return false;
        }
        public static bool Is<T>(this object? obj, [NotNullWhen(true)] out T? result)
        {
            if (obj is T typedObj && obj.IsNotNull())
            {
                result = typedObj;
                return true;
            }

            result = default;
            return false;
        }
        public static bool Is<TThis, T>(this TThis? obj, [NotNullWhen(true)] out T? result)
        {
            if (obj is T typedObj && obj.IsNotNull())
            {
                result = typedObj;
                return true;
            }

            result = default;
            return false;
        }

        public static bool IsNot<T>(this object? obj)
        {
            return !obj.Is<T>();
        }

        public static bool IsNot<TThis, T>(this TThis? obj)
        {
            return !obj.Is<TThis, T>();
        }

        public static bool IsNot<T>(this object? obj, [NotNullWhen(false)] out T? result)
        {
            return !obj.Is(out result);
        }

        public static bool IsNot<TThis, T>(this TThis? obj, [NotNullWhen(false)] out T? result)
        {
            return !obj.Is(out result);
        }

        public static bool IsNullOrType<T>(this object? source)
        {
            return source.IsNull() || source.Is<T>();
        }

        public static bool IsNullOrType<T>(this object? source, [NotNullWhen(true)] out T? result)
        {
            result = default;

            return source.IsNull() || source.Is(out result);
        }
    }
}
