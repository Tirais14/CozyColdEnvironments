using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

#nullable enable
namespace UTIRLib.Json
{
    public static class JsonSerializer
    {
        [SuppressMessage("Minor Bug", "S2955:Generic parameters not constrained to reference types should not be compared to \"null\"", Justification = "<Pending>")]
        public static string Serialize<T>(T obj, bool formatText = true)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (formatText)
            {
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }

            return JsonConvert.SerializeObject(obj);
        }

        /// <exception cref="StringArgumentException"></exception>
        /// <exception cref="DeserializeException"></exception>
        public static T Deserialize<T>(string serilized)
        {
            if (serilized.IsNullOrEmpty())
            {
                throw new StringArgumentException(nameof(serilized), serilized);
            }

            return JsonConvert.DeserializeObject<T>(serilized)
                ?? throw new DeserializeException();
        }
    }
}
