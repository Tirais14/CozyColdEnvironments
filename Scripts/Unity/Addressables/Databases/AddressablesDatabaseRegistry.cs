using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Unity.AddrsAssets.Databases;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public static AddressablesDatabaseRegistry Q => Instance;

        protected override void OnAwake()
        {
            base.OnAwake();

            try
            {
                var task = RegisterDatabases();
                CC.NeccesaryTasks.RegisterTask(task);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void RegisterDatabase(AssetRegistryKey key, IAddressablesDatabase database)
        {
            Instance.databases.Add(key, database);
        }

        public bool UnregisterDatabase(AssetRegistryKey key) => databases.Remove(key);

        public Object GetAsset(AssetRegistryKey key)
        {
            IAddressablesDatabase db = Instance.databases[key];

            return db.GetAsset(key.AssetKey);
        }

        public T GetAsset<T>(AssetRegistryKey key)
        {
            return GetAsset(key.With(typeof(T))).As<T>();
        }

        public void Dispose() => Dispose(disposing: true);

        public IEnumerator<IAddressablesDatabase> GetEnumerator()
        {
            return databases.Values.GetEnumerator();
        }

        protected abstract UniTask<KeyValuePair<AssetRegistryKey, IAddressablesDatabase>[]> GetDatabases();

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

        private async UniTask RegisterDatabases()
        {
            (await GetDatabases()).ForEach(db => RegisterDatabase(db.Key, db.Value));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
