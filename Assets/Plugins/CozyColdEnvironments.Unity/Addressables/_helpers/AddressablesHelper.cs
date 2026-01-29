using CCEnvs.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    public static class AddressablesHelper
    {
        public static void ReleasePrefabComponentAsset<T>(T cmp)
        {
            CC.Guard.IsNotNull(cmp, nameof(cmp));
            Addressables.Release(cmp.To<Component>().gameObject);
        }

        //public static void IsAssetLoaded(object asset)
        //{
        //    AsyncOperationHandle<Component> handle = Addressables.LoadAssetAsync<Component>();
        //}
    }
}
