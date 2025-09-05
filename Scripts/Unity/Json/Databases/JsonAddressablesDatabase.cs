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

            if (result.IsNull() && dto is ICCConvertible<TItem> convertible)
                result = convertible.Convert();

            if (result.IsNull())
                throw new TypeCastException(dto.GetType(), typeof(TItem));

            return result!;
        }

        protected override void ProccessTextAssets(IList<TextAsset> textAssets)
        {
            if (textAssets.IsNullOrEmpty())
            {
                CCDebug.PrintWarning($"{this.GetTypeName()}: Not loaded any textAsset.");
                return;
            }

            JsonSerializerSettings serializerSettings = GetSerializerSettings();
            Type[] deserializingTypes = ResolveDeserializedTypes();
            for (int i = 0; i < textAssets.Count; i++)
            {
                object deserialized = (from x in deserializingTypes
                                       select Deserialize(textAssets[i].text,
                                                          x,
                                                          serializerSettings) into d
                                       where d.IsNotNull()
                                       select d)
                                       .LastOrDefault() 
                                       ??
                                       throw new CCException($"Cannot be deserialized. Text = {textAssets[i].text}");

                TItem item = ConvertDeserialized(deserialized);

                values.Add(GetItemID(item), item);
            }

            //object? deserialized = null;
            //TItem? item;
            //int count = textAssets.Count;
            //JsonSerializerSettings serializerSettings = GetSerializerSettings();
            //Type deserializedType = ResolveDeserializedTypes();
            //for (int i = 0; i < count; i++)
            //{
            //    try
            //    {
            //        for (int j = 0; j < DeserializedTypes.Length; j++)
            //        {
            //            deserialized = JsonConvert.DeserializeObject(
            //                textAssets[i].text,
            //                deserializedType,
            //                serializerSettings);
            //        }
            //    }
            //    catch (JsonException) 
            //    {
            //        //Mock
            //    }

            //    if (deserialized.IsNull())
            //        throw new DeserializeDataException(deserializedType);

            //    item = ConvertDeserialized(deserialized);

            //    values.Add(GetItemID(item), item);
            //}

            values.TrimExcess();
        }

        protected abstract TId GetItemID(TItem item);

        protected abstract JsonConverter? GetConverter();

        private object? Deserialize(string text, Type type, JsonSerializerSettings settings)
        {
            try
            {
                return JsonConvert.DeserializeObject(text, type, settings);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private Type[] ResolveDeserializedTypes()
        {
            if (DeserializedTypes.IsNullOrEmpty())
                return CC.C.Array(typeof(TItem));

            return DeserializedTypes;
        }

        private JsonSerializerSettings GetSerializerSettings()
        {
            JsonConverter? converter = GetConverter();
            JsonSerializerSettings serializerSettings = JsonSettingsProvider.GetSettings();

            if (converter is null)
            {
                serializerSettings.Converters = JsonConverterHelper.RemoveByType(
                    serializerSettings.Converters,
                    typeof(TItem));
            }
            else
                serializerSettings.Converters = JsonConverterHelper.ReplaceByType(
                    serializerSettings.Converters,
                    converter);

            return serializerSettings;
        }
    }
}
