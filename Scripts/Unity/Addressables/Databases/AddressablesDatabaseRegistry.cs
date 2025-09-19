using CCEnvs.Linq;
using CCEnvs.Unity.AddrsAssets.Databases;
using System.Collections.Generic;
using Object = UnityEngine.Object;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.AddrsAssets
{
    public abstract class AddressablesDatabaseRegistry
        : MonoCCStatic<AddressablesDatabaseRegistry>,
        IAddressablesDatabaseRegistry
    {
        private readonly Dictionary<AssetRegistryKey, IAddressablesDatabase> databases = new();
        private bool disposedValue;

        public Object GetAsset(AssetRegistryKey key)
        {
            IAddressablesDatabase db = databases[key];

            return db.GetAsset(key.AssetKey);
        }

        public T GetAsset<T>(AssetRegistryKey key)
        {
            return GetAsset(key.With(typeof(T))).As<T>();
        }

        public void Dispose() => Dispose(disposing: true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) 
                return;

            if (disposing)
            {
                databases.Values.ForEach(x => x.Dispose());
                databases.Clear();
                databases.TrimExcess();
            }

            disposedValue = true;
        }
    }
}
