using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public struct StringOrNumber
    {
        [SerializeField]
        private string _value;

        public readonly string Value => _value;
        public readonly int? Number {
            get
            {
                if (int.TryParse(_value, out var number))
                    return number;

                return null;
            }
        }
        public readonly bool IsNumber => Number is not null;

        public StringOrNumber(string value)
        {
            _value = value;
        }

        public static explicit operator int?(StringOrNumber source)
        {
            return source.Number;
        }

        public readonly override string ToString() => _value;
    }
}
