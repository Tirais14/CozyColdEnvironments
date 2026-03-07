using System;
using CCEnvs.Json;
using CCEnvs.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

#nullable enable
namespace CCEnvs.Saves.Json
{
    public class SaveEntryJsonConverter : JsonConverter<SaveEntry>
    {
        public override SaveEntry ReadJson(
            JsonReader reader,
            Type objectType,
            SaveEntry existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
            )
        {
            var jObj = JObject.Load(reader);

            var instance = JsonSerializerInternalReaderHelper.CreateNewObject(objectType, jObj.CreateReader(), serializer);

            serializer.Populate(jObj.CreateReader(), instance);

            var saveEntry = (SaveEntry)instance;

            if (serializer.Context.Context is not SaveSystemStreamingContext saveSysCtx)
                return saveEntry;

            if (saveSysCtx.SaveDataVersion > saveEntry.Version)
            {
                for (long nextVersion = saveEntry.Version + 1; nextVersion < saveSysCtx.SaveDataVersion; nextVersion++)
                    SaveEntryVersionHandler.TryUpgrade(saveEntry, nextVersion, out saveEntry);
            }

            return saveEntry;
        }

        public override void WriteJson(
            JsonWriter writer,
            SaveEntry value,
            JsonSerializer serializer
            )
        {
            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(TypeofCache<SaveEntry>.Type);

            writer.WriteStartObject();
            JsonSerializerInternalWriterHelper.WriteObjectBody(writer, contract, serializer, value);
            writer.WriteEndObject();
        }
    }
}
