#nullable enable
namespace CCEnvs
{
    //public readonly struct EnumWrapperInt<TEnum> 
    //    :
    //    IEquatable<EnumWrapperInt<TEnum>>, 
    //    IEquatable<TEnum>,
    //    IEquatable<int>

    //    where TEnum : unmanaged, Enum
    //{
    //    public readonly TEnum Value;

    //    public readonly int UnderlyingValue;

    //    public EnumWrapperInt(int underlyingValue)
    //    {
    //        Value = underlyingValue.CastTo<TEnum>();
    //        UnderlyingValue = underlyingValue;
    //    }

    //    public EnumWrapperInt(TEnum value)
    //    {
    //        Value = value;
    //        UnderlyingValue = value.ToIntUnsafe();
    //    }

    //    public bool Equals(TEnum other) => UnderlyingValue == other.ToIntUnsafe();

    //    public bool Equals(int other) => UnderlyingValue == other;

    //    public bool Equals(EnumWrapperInt<TEnum> other) => Equals(other.Value);

    //    public override bool Equals(object obj)
    //    {
    //        return obj switch
    //        {
    //            EnumWrapperInt<TEnum> wrapper => Equals(wrapper.Value),
    //            TEnum enm => Equals(enm),
    //            int num => Equals(num),
    //            _ => false
    //        };
    //    }

    //    public override int GetHashCode() => Value.GetHashCode();

    //    public override string ToString() => Value.ToString();
    //}
}
