using System;
using System.Diagnostics;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    [DebuggerDisplay("HashCode: {GetHashCode()}; String {ToString()}")]
    public readonly struct AssetKey : IEquatable<AssetKey>
    {
        public string? AssetName { get; }
        public int? AssetID { get; }
        public object? UniqueIdentifier { get; }

        public AssetKey(string? assetName, int? assetID) : this()
        {
            AssetName = assetName;
            AssetID = assetID;
        }

        public AssetKey(string? objName,
                        int? objID,
                        object? uniqueIndentifier)
            :
            this(objName, objID)
        {
            UniqueIdentifier = uniqueIndentifier;
        }

        public AssetKey(string objName, object? uniqueIndentifier = null)
            :
            this(objName, objID: default, uniqueIndentifier)
        {
        }

        public AssetKey(int objID, object? uniqueIndentifier = null)
            :
            this(objName: null, objID, uniqueIndentifier)
        {
        }

        public AssetKey(Object asset, object? uniqueIndentifier)
            :
            this()
        {
            CC.Validate.ArgumentNull(asset, nameof(asset));

            AssetName = asset.name;

            if (asset is IIDMarked<int> idMarked)
                AssetID = idMarked.ID;

            UniqueIdentifier = uniqueIndentifier;
        }

        public AssetKey(Object asset)
            :
            this(asset, uniqueIndentifier: null)
        {

        }

        public static bool operator ==(AssetKey left, AssetKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetKey left, AssetKey right)
        {
            return !(left == right);
        }

        public AssetKey With(string? assetName)
        {
            return new AssetKey(assetName, AssetID, UniqueIdentifier);
        }
        public AssetKey With(int? assetID)
        {
            return new AssetKey(AssetName, assetID, UniqueIdentifier);
        }
        public AssetKey With(object? uniqueIdentifier)
        {
            return new AssetKey(AssetName, AssetID, uniqueIdentifier);
        }

        public bool Equals(AssetKey other)
        {
            return AssetName == other.AssetName
                   &&
                   AssetID == other.AssetID
                   &&
                   UniqueIdentifier == other.UniqueIdentifier;
        }
        public override bool Equals(object obj)
        {
            return obj is AssetKey typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AssetName, AssetID, UniqueIdentifier);
        }

        public override string ToString()
        {
            return $"{nameof(AssetName)}: {AssetName} | {nameof(AssetID)}: {AssetID} | {nameof(UniqueIdentifier)}: {UniqueIdentifier}";
        }
    }
}
