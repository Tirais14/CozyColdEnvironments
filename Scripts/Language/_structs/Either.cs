#nullable enable
using CCEnvs.Language;
using System;
using System.Collections.Generic;

namespace CCEnvs
{
    //public readonly struct Either<L, R> : IEquatable<Either<L, R>>
    //{
    //    public static Either<L, R> Default => new();

    //    private readonly L left;
    //    private readonly R right;

    //    public object Value { get; }
    //    public bool HasValue => Value.IsNotDefault();

    //    public Either(L left, R right)
    //    {
    //        if (getLeft)
    //        {

    //        }    

    //        this.right = right.ToOption();
    //    }

    //    public static implicit operator T(Option<T> source)
    //    {
    //        return source.Value!;
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
