#nullable enable
using System;
using System.Collections.Generic;

namespace CCEnvs.Language
{
    //public readonly struct Option<T> : IOptional<T>, IEquatable<Option<T>>
    //{
    //    public static Option<T> Default => new();

    //    public T? DefaultValue { get; }
    //    public T Value { get; }
    //    public bool HasValue => Value.IsNotDefault();

    //    public Option(T? value)
    //    {
    //        Value = value!;
    //        DefaultValue = default;
    //    }

    //    public Option(T? value, T? @default)
    //        :
    //        this(value)
    //    {
    //        DefaultValue = @default;
    //    }

    //    public static implicit operator T(Option<T> source)
    //    {
    //        return source.Value!;
    //    }

    //    public static implicit operator bool(Option<T> source)
    //    {
    //        return source.HasValue;
    //    }

    //    public static bool operator ==(Option<T> left, Option<T> right)
    //    {
    //        return left.Equals(right);
    //    }

    //    public static bool operator !=(Option<T> left, Option<T> right)
    //    {
    //        return !(left == right);
    //    }

    //    public Option<T> Set(T value) => new(value);

    //    public Option<T> Reset() => new(Default);

    //    public override string ToString()
    //    {
    //        return $"{nameof(Value)}: {Value}; {nameof(DefaultValue)}: {DefaultValue}; {nameof(HasValue)}: {HasValue}";
    //    }

    //    public bool Equals(Option<T> other)
    //    {
    //        var comparer = EqualityComparer<T>.Default;

    //        return comparer.Equals(Value!, other.Value!)
    //               &&
    //               comparer.Equals(DefaultValue!, other.DefaultValue!);
    //    }
    //    public override bool Equals(object obj)
    //    {
    //        return obj is Option<T> typed && Equals(typed);  
    //    }

    //    public override int GetHashCode()
    //    {
    //        return HashCode.Combine(DefaultValue, Value);
    //    }
    //}
}
