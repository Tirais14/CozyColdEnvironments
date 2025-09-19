using CCEnvs.Unity.AddrsAssets.Databases;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.AddressableAssets.Databases
{
    public abstract class TextAddressablesDatabase : AddressablesDatabase<TextAsset>
    {
        protected abstract void ProccessTextAssets(IList<TextAsset> textAssets);

        protected override void OnLoaded(IList<TextAsset> assets)
        {
            ProccessTextAssets(assets);
        }
    }
}
