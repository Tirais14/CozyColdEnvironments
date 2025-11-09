using System;
using UnityEngine;

#nullable enable
#pragma warning disable IDE1006
#pragma warning disable S101
namespace CCEnvs.FuncLanguage
{
    public readonly struct nil : IEquatable<nil>
    {
        public readonly static nil it;

        public static bool operator ==(nil left, nil right) => true;

        public static bool operator !=(nil left, nil right) => false;

        public static bool operator ==(nil left, object? right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(nil left, object? right)
        {
            return !(left == right);
        }

        public bool Equals(nil other) => true;
        public override bool Equals(object? obj) => obj is null || obj is nil;

        public override int GetHashCode() => 0;

        public override string ToString()
        {
            return "null";
        }
    }
}
