
using CCEnvs.Diagnostics;
using CCEnvs.Unity.AddrsAssets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Json.AddressableAssets.Databases
{
    public abstract class JsonAddressablesDatabase<T> 
        : AddressablesDatabase<T>
    {
        protected override void ProccessTextAssets(IList<TextAsset> textAssets)
        {
            if (textAssets.IsNullOrEmpty())
            {
                CCDebug.PrintWarning("Not loaded any textAsset.", this);
                return;
            }

            JsonSerializerSettings serializerSettings = GetSerializerSettings();
            T deserialized = default!;
            for (int i = 0; i < textAssets.Count; i++)
            {
                try
                {
                    deserialized = JsonConvert.DeserializeObject<T>(
                        textAssets[i].text,
                        serializerSettings)
                        ??
                        throw new CCException($"Error while deserializng object. Type = {typeof(T)}, data = {textAssets[i]}");
                }
                catch (InvalidCastException ex)
                {
                    CC.Throw.InvalidCast(typeof(T), "Most likely the polymorph coverter returned wrong value type", ex);
                }

                values.Add(GetItemID(deserialized), deserialized);
            }

            values.TrimExcess();
        }

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
                    typeof(T));
            }

            return serializerSettings;
        }
    }
}
