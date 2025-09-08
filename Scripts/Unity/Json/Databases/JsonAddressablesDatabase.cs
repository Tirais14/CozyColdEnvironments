using CCEnvs.AddressableAssets.Databases;
using CCEnvs.Common;
using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        protected abstract Type[] DeserializedTypes { get; }

        private static TItem ConvertDeserialized(object dto)
        {
            var result = dto.IsQ<TItem>();

            if (result.IsNull() && dto is IConvertibleCC<TItem> convertible)
                result = convertible.Convert();

            if (result.IsNull())
                Throw.InvalidCast(dto.GetType(), typeof(TItem));

            return result!;
        }

        private static object? Deserialize(string text,
                                           Type type,
                                           JsonSerializerSettings settings)
        {
            try
            {
                CCDebug.PrintLog($"Deserializing => {type.GetName()}",
                    DebugContext.Additive(typeof(JsonAddressablesDatabase<TId, TItem>)));

                var obj = JsonConvert.DeserializeObject(text, type, settings);

                return obj;
            }
            catch (JsonException ex)
            {
                CCDebug.PrintExceptionAsLog(ex,
                    DebugContext.Additive(typeof(JsonAddressablesDatabase<TId, TItem>)));
                return null;
            }
        }

        protected override void ProccessTextAssets(IList<TextAsset> textAssets)
        {
            if (textAssets.IsNullOrEmpty())
            {
                CCDebug.PrintWarning("Not loaded any textAsset.", this);
                return;
            }

            JsonSerializerSettings serializerSettings = GetSerializerSettings();
            Type[] deserializingTypes = ResolveDeserializedTypes();
            for (int i = 0; i < textAssets.Count; i++)
            {
                //Iterates through the different types.
                //In high priority last not null object.
                //Last added type override the same early added
                object deserialized = (from type in deserializingTypes
                                       select Deserialize(textAssets[i].text,
                                                          type,
                                                          serializerSettings) into deserializedTemp
                                       where deserializedTemp.IsNotNull()
                                       select deserializedTemp)
                                       .LastOrDefault() 
                                       ??
                                       throw new CCException($"Cannot be deserialized. Text = {textAssets[i].text}");

                TItem item = ConvertDeserialized(deserialized);

                values.Add(GetItemID(item), item);
            }

            values.TrimExcess();
        }

        protected abstract TId GetItemID(TItem item);

        protected virtual object? GetConverter() => CC.EmptyObject;

        private Type[] ResolveDeserializedTypes()
        {
            if (DeserializedTypes.IsNullOrEmpty())
                return CC.C.Array(typeof(TItem));

            return DeserializedTypes;
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
                    typeof(TItem));
            }

            return serializerSettings;
        }
    }
}
