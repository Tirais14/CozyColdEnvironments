using System.Collections.Generic;

#nullable enable

namespace UTIRLib
{
    public readonly struct ValuePair : IValuePair
    {
        public readonly object First { get; }
        public readonly object Second { get; }

        public ValuePair(object first, object second)
        {
            First = first;
            Second = second;
        }
    }

    public readonly struct ValuePair<TFirst, TSecond> : IValuePair<TFirst, TSecond>
    {
        public readonly TFirst First { get; }
        public readonly TSecond Second { get; }

        public ValuePair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }

        public static explicit operator KeyValuePair<TFirst, TSecond>(ValuePair<TFirst, TSecond> keyValuePair)
        {
            return new(keyValuePair.First, keyValuePair.Second);
        }

        object IValuePair.First => First!;
        object IValuePair.Second => Second!;
    }
}