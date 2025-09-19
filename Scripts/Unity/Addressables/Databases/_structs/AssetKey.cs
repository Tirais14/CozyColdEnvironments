using System;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public readonly struct AssetKey : IEquatable<AssetKey>
    {
        private readonly string objName;
        private readonly int objID;
        private readonly object? uniqueIndentifier;

        public AssetKey(string objName, int objID) : this()
        {
            this.objName = objName;
            this.objID = objID;
        }

        public AssetKey(string objName,
                        int objID,
                        object? uniqueIndentifier)
            :
            this(objName, objID)
        {
            this.uniqueIndentifier = uniqueIndentifier;
        }

        public AssetKey(Object asset, object? uniqueIndentifier)
        {
            CC.Validate.ArgumentNull(asset, nameof(asset));

            objName = asset.name;

            if (asset is IIDMarked idMarked)
                objID = idMarked.ObjectID;
            else
                objID = int.MinValue;

            this.uniqueIndentifier = uniqueIndentifier;
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

        public bool Equals(AssetKey other)
        {
            return ToString() == other.ToString();
        }
        public override bool Equals(object obj)
        {
            return obj is AssetKey typed && Equals(typed);
        }

        public override int GetHashCode() => ToString().GetHashCode();

        public override string ToString()
        {
            return $"Object name = {objName}; object ID = {objID}; unique indentifier = {uniqueIndentifier}";
        }
    }
}
