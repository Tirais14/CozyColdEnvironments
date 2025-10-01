using System;
using UnityEngine;

#nullable enable
#pragma warning disable S1450
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public struct StringOrInteger
    {
        [SerializeField]
        private string _value;
        private int? number;
        private bool isParsed;

        public readonly string Value => _value;
        public int? Number {
            get
            {
                if (isParsed)
                    return number;

                if (int.TryParse(_value, out var parsed))
                    number = parsed;

                isParsed = true;
                return number;
            }
        }
        public bool IsNumber => Number is not null;

        public StringOrInteger(string value) : this()
        {
            _value = value;
        }

        public static explicit operator int?(StringOrInteger source)
        {
            return source.Number;
        }
        public static explicit operator int(StringOrInteger source)
        {
            return source.Number.GetValueOrDefault();
        }

        public readonly override string ToString() => _value;
    }
}
