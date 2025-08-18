using Newtonsoft.Json;
using System;
using UnityEditor;
using UTIRLib.Json.DTO;

#nullable enable
namespace UTIRLib.Json.Converters
{
    public class AddressableConverter<T> : JsonConverter<T>
        where T : UnityEngine.Object
    {
        public override T? ReadJson(JsonReader reader,
                                    Type objectType,
                                    T? existingValue,
                                    bool hasExistingValue,
                                    JsonSerializer serializer)
        {
            var dto = serializer.Deserialize<AddressableDTO<T>>(reader);

            if (dto is null)
                return null;

            dto.StartAssetLoading();

            return dto.Asset;
        }

        public override void WriteJson(JsonWriter writer,
                                       T? value,
                                       JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(value);
            serializer.Serialize(writer, new AddressableDTO<T>
            {
                AssetPath = assetPath,
                GUID = AssetDatabase.AssetPathToGUID(assetPath)
            });
#else      
            throw new System.NotImplementedException("Serializing assets in runtime not implemented");
#endif
        }
    }
}
