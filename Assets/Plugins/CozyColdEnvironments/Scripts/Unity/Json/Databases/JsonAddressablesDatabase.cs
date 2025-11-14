using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Unity.AddrsAssets.Databases;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Json.AddressableAssets.Databases
{
    public abstract class JsonAddressablesDatabase<TKey, TValue> 
        : Database<TKey, TValue>
    {
        public async UniTask LoadAssetsAsync(string[] labels,
            Func<TValue, TKey> keySelector)
        {
            throw new NotImplementedException();
            //CC.Guard.IsNotNull(keySelector, nameof(keySelector));

            //using var textAssets = new AddressablesDatabase<TextAsset>();

            //await textAssets!.LoadAssetsByLabelsAsync<TextAsset>(labels);

            //textAssets.Values.CForEach(item => Deserialize(item, keySelector));

            //TrimExcess();
        }

        /// <returns><see langword="null"/> for deleting any converter of <see langword="TItem"/> or value for override converter</returns>
        protected virtual object? GetConverter() => CC.EmptyObject;

        private void Deserialize(TextAsset textAsset, Func<TValue, TKey> keySelector)
        {
            try
            {
                JsonSerializerSettings serializerSettings = GetSerializerSettings();

                TValue deserialized = JsonConvert.DeserializeObject<TValue>(
                    textAsset.text,
                    serializerSettings)
                    ??
                    throw new CCException($"Error while deserializng object. Type = {typeof(TValue)}, data = {textAsset}");

                Add(keySelector(deserialized), deserialized);
            }
            catch (InvalidCastException ex)
            {
                CC.Throw.InvalidCast(typeof(TValue), "Most likely the polymorph coverter returned wrong value type", ex);
            }
        }

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
                    typeof(TValue));
            }

            return serializerSettings;
        }
    }
}
