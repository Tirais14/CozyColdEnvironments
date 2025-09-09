using CCEnvs.AddressableAssets.Databases;
using CCEnvs.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Json.AddressableAssets.Databases
{
    public abstract class JsonAddressablesDatabase<TId, TItem> 
        : TextAddressablesDatabase
    {
        private readonly Dictionary<TId, TItem> values = new();

        protected IReadOnlyDictionary<TId, TItem> db => values;

        protected override void ProccessTextAssets(IList<TextAsset> textAssets)
        {
            if (textAssets.IsNullOrEmpty())
            {
                CCDebug.PrintWarning("Not loaded any textAsset.", this);
                return;
            }

            JsonSerializerSettings serializerSettings = GetSerializerSettings();
            for (int i = 0; i < textAssets.Count; i++)
            {
                var deserialized = JsonConvert.DeserializeObject<TItem>(
                    textAssets[i].text,
                    serializerSettings)
                    ??
                    throw new CCException($"Error while deserializng object. Type = {typeof(TItem)}, data = {textAssets[i]}");

                values.Add(GetItemID(deserialized), deserialized);
            }

            values.TrimExcess();
        }

        protected abstract TId GetItemID(TItem item);

        /// <returns><see langword="null"/> for deleting any converter of <see langword="TItem"/> or value for override converter</returns>
        protected virtual object? GetConverter() => CC.EmptyObject;

        private JsonSerializerSettings GetSerializerSettings()
        {
            object? converter = GetConverter();
            JsonSerializerSettings serializerSettings = JsonSettingsProvider.GetSettings();

            if (converter is JsonConverter converterTyped)
            {
                serializerSettings.Converters = JsonConverterCollectionHelper.ReplaceByType(
                    serializerSettings.Converters,
                    converterTyped);
            }
            else if (converter is null)
            {
                serializerSettings.Converters = JsonConverterCollectionHelper.RemoveByType(
                    serializerSettings.Converters,
                    typeof(TItem));
            }

            return serializerSettings;
        }
    }
}
