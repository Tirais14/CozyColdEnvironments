using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

#nullable enable

namespace UTIRLib.FileSystem.Json.Convertes
{
    public class SpriteJsonConverter : JsonConverter<Sprite>
    {
        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override void WriteJson(JsonWriter writer, Sprite? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Task<IList<IResourceLocation>> task = Addressables.LoadResourceLocationsAsync(value).Task;
            task.RunSynchronously();
            serializer.Serialize(writer, task.Result.First()?.PrimaryKey);
        }

        public override Sprite? ReadJson(JsonReader reader, Type objectType, Sprite? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            Task<Sprite> task = Addressables.LoadAssetAsync<Sprite>(serializer.Deserialize(reader)).Task;
            task.RunSynchronously();
            return task.Result;
        }
    }
}