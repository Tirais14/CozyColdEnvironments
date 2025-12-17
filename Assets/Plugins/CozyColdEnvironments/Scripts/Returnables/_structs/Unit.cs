#nullable enable
using System;

namespace CCEnvs.Returnables
{
    public readonly struct Unit : IEquatable<Unit>
    {
        public static Unit Default { get; } = new();

        public static bool operator ==(Unit left, Unit right) => true;

        public static bool operator !=(Unit left, Unit right) => false;

        public bool Equals(Unit other) => this == other;

        public override bool Equals(object obj) => obj is Unit;

        public override int GetHashCode() => 0;

        public override string ToString() => "()";
    }
}
