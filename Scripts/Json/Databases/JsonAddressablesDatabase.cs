using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UTIRLib.AddressableAssets.Databases;

#nullable enable
#pragma warning disable IDE1006
namespace UTIRLib.Json.AddressableAssets.Databases
{
    public abstract class JsonAddressablesDatabase<TId, TItem> : TextAddressablesDatabase
    {
        private Dictionary<TId, TItem> values = null!;

        protected IReadOnlyDictionary<TId, TItem> db => values;

        protected override void ProccessTextAssets(IList<TextAsset> textAssets)
        {
            if (textAssets.IsNullOrEmpty())
            {
                TirLibDebug.PrintWarning("Not loaded any textAsset.");
                return;
            }

            values = new Dictionary<TId, TItem>(textAssets.Count);

            TItem deserialized;
            int count = textAssets.Count;
            for (int i = 0; i < count; i++)
            {
                deserialized = JsonConvert.DeserializeObject<TItem>(
                    textAssets[i].text,
                    JsonSerializedSettingsProvider.Converters)!;

                values.Add(GetItemID(deserialized), deserialized);

                if (deserialized is null)
                    TirLibDebug.PrintError("Database cannot contain null items.");
            }
        }

        protected abstract TId GetItemID(TItem item);
    }
}
