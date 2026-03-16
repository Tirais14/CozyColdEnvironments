using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ObservableCollections;
using System;
using System.Collections;

#nullable enable
namespace CCEnvs.Saves.Json
{
    public class SaveEntriesJsonConverter
        :
        JsonConverter<ObservableDictionary<string, SaveEntry>>
    {
        public override bool CanWrite => false;

        public override ObservableDictionary<string, SaveEntry>? ReadJson(
            JsonReader reader,
            Type objectType,
            ObservableDictionary<string, SaveEntry>? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
            )
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jObj = JObject.Load(reader);

            ObservableDictionary<string, SaveEntry> dictionary;

            if (existingValue is not null)
                dictionary = existingValue;
            else
                dictionary = new ObservableDictionary<string, SaveEntry>(jObj.Count, comparer: null);

            string key;

            SaveEntry entry;

            foreach (var jProp in jObj.Properties())
            {
                try
                {
                    key = jProp.Name;

                    entry = jProp.Value.ToObject<SaveEntry>(serializer);

                    dictionary.Add(key, entry);
                }
                catch (Exception ex)
                {
                    SaveEntry? tEntry = SaveSystemErrorHandler._onSaveEntryDeserializingError?.Invoke(jProp, ex);

                    if (tEntry.HasValue)
                    {
                        entry = tEntry.Value;

                        dictionary.Add(entry.Key, entry);
                    }
                }
            }

            return dictionary;
        }

        public override void WriteJson(
            JsonWriter writer,
            ObservableDictionary<string, SaveEntry>? value,
            JsonSerializer serializer
            )
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            foreach (var item in value.To<IEnumerable>())
                serializer.Serialize(writer, item);

            writer.WriteEndObject();
        }
    }
}
