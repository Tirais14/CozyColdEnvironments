using CCEnvs.Linq;
using CCEnvs.Unity.EditorSerialization;
using System;
using System.Linq;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets.Databases
{
    [Serializable]
    public struct AssetLabels : IEquatable<AssetLabels>
    {
        [field: SerializeField]
        public SerializedImmutableArray<string> Labels { get; private set; }

        [field: SerializeField]
        public bool MustBeAll { get; private set; }

        public AssetLabels(bool mustBeAll, params string[] labels) : this()
        {
            Labels = new SerializedImmutableArray<string>(labels);
            MustBeAll = mustBeAll;
        }
        public AssetLabels(params string[] labels)
            :
            this(mustBeAll: true, labels)
        {
        }

        public static bool operator ==(AssetLabels left, AssetLabels right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetLabels left, AssetLabels right)
        {
            return !(left == right);
        }

        public static implicit operator string[](AssetLabels assetLabels)
        {
            return assetLabels.Labels.Value.ToArray();
        }

        public readonly string[] ToArray() => Labels.Value.ToArray();

        public readonly bool Equals(AssetLabels other)
        {
            if (Labels == other.Labels)
                return true;

            return Labels is not null
                   &&
                   other.Labels is not null
                   && 
                   Labels.Value.AsValueEnumerable().SequenceEqual(other.Labels.Value);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is AssetLabels typed && Equals(typed);
        }

        public readonly override int GetHashCode() => Labels.Value.SequenceToHashCode();

        public readonly override string ToString()
        {
            return $"{nameof(Labels)}: {Labels.Value.JoinStrings(", ")}";
        }
    }
}
