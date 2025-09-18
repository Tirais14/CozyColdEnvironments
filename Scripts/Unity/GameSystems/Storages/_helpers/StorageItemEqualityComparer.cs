using CCEnvs.Diagnostics;
using CCEnvs.Unity.GameSystems.Storages;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public class StorageItemEqualityComparer : IEqualityComparer<IStorageItem>
    {
        public bool Equals(IStorageItem x, IStorageItem y)
        {
            if (x.IsNull() || y.IsNull())
                return false;
            if (x.IsNull() && y.IsNull())
                return true;

            return x.ID == y.ID;
        }

        public int GetHashCode(IStorageItem obj) => obj.ID.GetHashCode();
    }
}
