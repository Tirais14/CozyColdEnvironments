using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using CozyColdEnvironments.AddressableAssets.Databases;

#nullable enable
#pragma warning disable IDE1006
namespace CozyColdEnvironments.Json.AddressableAssets.Databases
{
    public abstract class JsonAddressablesDatabase<TId, TItem> : TextAddressablesDatabase
    {
        private readonly Dictionary<TId, TItem> values = new();

        protected IReadOnlyDictionary<TId, TItem> db => values;

        protected override void ProccessTextAssets(IList<TextAsset> textAssets)
        {
            if (textAssets.IsNullOrEmpty())
            {
                TirLibDebug.PrintWarning($"{this.GetTypeName()}: Not loaded any textAsset.");
                return;
            }

            TItem deserialized;
            int count = textAssets.Count;
            for (int i = 0; i < count; i++)
            {
                deserialized = JsonConvert.DeserializeObject<TItem>(
                    textAssets[i].text,
                    JsonSerializedSettingsProvider.GetSettings())!;

                values.Add(GetItemID(deserialized), deserialized);

                if (deserialized is null)
                    TirLibDebug.PrintError("Database cannot contain null items.");
            }

            values.TrimExcess();
        }

        protected abstract TId GetItemID(TItem item);
    }
}
