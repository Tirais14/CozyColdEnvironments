using System;
using Newtonsoft.Json;
using UTIRLib.Linq;

#nullable enable
namespace UTIRLib.FileSystem.Json
{
    public class JsonFile : FileEntry
    {
        public override string Extension {
            get => ".json";
            set => _ = value;
        }

        public JsonFile(params string[] pathParts) : base(pathParts)
        {
        }

        public JsonFile(FSPath path) : this(path.value)
        {
        }

        public override void SetPath(FSPath path)
        {
            if (path.Extension != Extension)
            {
                this.path = path.WithExtension(Extension);
                return;
            }

            this.path = path;
        }

        public void Serialize(object toSerialize, bool format = true)
        {
            customContent = JsonConvert.SerializeObject(toSerialize,
                format ? Formatting.Indented : Formatting.None);
        }
        public void Serialize(object toSerialize, JsonSerializerSettings settings)
        {
            customContent = JsonConvert.SerializeObject(toSerialize, settings);
        }

        public object? Deserialize(Type type, JsonSerializerSettings? settings = null)
        {
            string text = ReadText();

            if (settings is null)
            {
                return JsonConvert.DeserializeObject(text, type);
            }
            else
            {
                return JsonConvert.DeserializeObject(text, type, settings);
            }
        }

        public T? Deserialize<T>(JsonSerializerSettings? settings = null)
        {
            return Deserialize(typeof(T), settings).IsQ<T>();
        }
    }
}