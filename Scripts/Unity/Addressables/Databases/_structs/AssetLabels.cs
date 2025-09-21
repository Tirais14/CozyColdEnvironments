using CCEnvs.Linq;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    [Serializable]
    public struct AssetLabels : IEquatable<AssetLabels>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private string[]? labels;

        public ReadOnlyCollection<string> Labels { get; private set; }

        public AssetLabels(params string[] labels) : this()
        {
            Labels = new ReadOnlyCollection<string>(labels);
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
            return assetLabels.Labels.ToArray();
        }

        public readonly string[] ToArray() => Labels.ToArray();

        public readonly bool Equals(AssetLabels other)
        {
            if (Labels == other.Labels)
                return true;

            return Labels is not null
                   &&
                   other.Labels is not null
                   && 
                   Labels.SequenceEqual(other.Labels);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is AssetLabels typed && Equals(typed);
        }

        public readonly override int GetHashCode() => Labels.ToHashCode();

        public readonly override string ToString()
        {
            return $"{nameof(Labels)}: {Labels.JoinStrings(", ")}";
        }

        readonly void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            CC.DoNothing();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Labels = new ReadOnlyCollection<string>(labels);
        }
    }
}
