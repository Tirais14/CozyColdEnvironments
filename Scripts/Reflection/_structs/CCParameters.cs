using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable enable
namespace CCEnvs.Reflection.Data
{
    public struct CCParameters
      :
      IReadOnlyList<CCParameterInfo>,
      IEquatable<CCParameters>
    {
        public static CCParameters Empty => new(Array.Empty<CCParameterInfo>());

        public ReadOnlyCollection<CCParameterInfo> Values { get; }
        public bool IgnoreOptionalInEquals { get; set; }
        public readonly CCParameterInfo this[int index] => Values[index];
        public readonly int Count => Values.Count;

        public CCParameters(params CCParameterInfo[] parameters) : this()
        {
            Values = new ReadOnlyCollection<CCParameterInfo>(parameters);
        }

        public readonly CCParameterInfo[] GetRequiredParameters()
        {
            return Values.Where(x => !x.HasDefaultValue).ToArray();
        }
        
        public static explicit operator Type[](CCParameters parameters)
        {
            return parameters.Select(x => x.ParameterType).ToArray();
        }

        public static bool operator ==(CCParameters left, CCParameters right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CCParameters left, CCParameters right)
        {
            return !left.Equals(right);
        }

        public readonly bool Equals(CCParameters other)
        {
            bool result = Values.SequenceEqual(other.Values);

            if (!result && IgnoreOptionalInEquals)
                result = GetRequiredParameters().SequenceEqual(other.GetRequiredParameters());

            return result;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is CCParameters typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Values, IgnoreOptionalInEquals);
        }

        public readonly IEnumerator<CCParameterInfo> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }
    }
    //public readonly struct Parameters
    //    :
    //    IReadOnlyList<Type>,
    //    IEquatable<Parameters>
    //{
    //    public static Parameters Empty { get; } = new Parameters(Type.EmptyTypes);

    //    private readonly ReadOnlyCollection<Type> types;

    //    public IReadOnlyList<Type> Types => types;
    //    public int Count => types?.Count ?? 0;
    //    public Type this[int index] => types[index];

    //    public Parameters(params Type[] types)
    //    {
    //        this.types = new ReadOnlyCollection<Type>(types);
    //    }
    //    public Parameters(IEnumerable<Type> types)
    //        :
    //        this(types.ToArray())
    //    {
    //    }

    //    public static bool operator ==(Parameters left, Parameters right)
    //    {
    //        return left.Equals(right);
    //    }
    //    public static bool operator !=(Parameters left, Parameters right)
    //    {
    //        return !left.Equals(right);
    //    }

    //    public static explicit operator Type[](Parameters signature)
    //    {
    //        return signature.types?.ToArray() ?? Type.EmptyTypes;
    //    }

    //    public bool Equals(Parameters other)
    //    {
    //        if (types is null && other.types is null)
    //            return true;
    //        if (other.types is null)
    //            return false;

    //        return types.SequenceEqual(other.types);
    //    }
    //    public override bool Equals(object obj)
    //    {
    //        return obj is Parameters typed && Equals(typed);
    //    }

    //    public override int GetHashCode()
    //    {
    //        var hash = new HashCode();
    //        for (int i = 0; i < types.Count; i++)
    //            hash.Add(types[i]);

    //        return hash.ToHashCode();
    //    }

    //    public override string ToString()
    //    {
    //        if (((ICollection<Type>)types).IsNullOrEmpty())
    //            return "empty";

    //        var builder = new StringBuilder();
    //        for (int i = 0; i < types.Count; i++)
    //            builder.Append($"Position = {i}, type = {types[i].GetName()}; ");

    //        return builder.ToString();
    //    }

    //    public IEnumerator<Type> GetEnumerator()
    //    {
    //        return types?.GetEnumerator() ?? TEnumerable<Type>.Empty.GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }
    //}
}
