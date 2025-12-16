using CommunityToolkit.Diagnostics;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Json
{
    public static class JsonSerializerOptionsHelper
    {
        public static JsonSerializerOptions ExcludeConverters(
            this JsonSerializerOptions source,
            params Type[] converterTypes)
        {
            Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(converterTypes, nameof(converterTypes));

            if (source.IsReadOnly || source.Converters.IsReadOnly)
            {
                var options = new JsonSerializerOptions(source);
                foreach (var conv in source.Converters)
                {
                    if (!isToRemove(conv, converterTypes))
                        continue;

                    options.Converters.Remove(conv);
                }

                return options;
            }

            using var converters = source.Converters.ToArrayPooled();
            foreach (var conv in converters.Value)
            {
                if (!isToRemove(conv, converterTypes))
                    continue;

                source.Converters.Remove(conv);
            }

            return source;

            static bool isToRemove(JsonConverter conv, Type[] toRemoveTypes)
            {
                Type convType = conv.GetType();
                foreach (var type in toRemoveTypes)
                {
                    if (type == convType)
                        return true;
                }

                return false;
            }
        }

        public static JsonSerializerOptions ExcludeConverters(
            this JsonSerializerOptions source,
            params JsonConverter[] converters)
        {
            Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(converters, nameof(converters));

            return source.ExcludeConverters(converters.Select(x => x.GetType()).ToArray());
        }

        public static JsonSerializerOptions AddConverters(
            this JsonSerializerOptions source,
            params JsonConverter[] converters)
        {
            Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(converters, nameof(converters));

            if (source.IsReadOnly || source.Converters.IsReadOnly)
            {
                var options = new JsonSerializerOptions(source);
                foreach (var conv in converters)
                    options.Converters.Add(conv);

                return options;
            }

            foreach (var conv in converters)
                source.Converters.Add(conv);

            return source;
        }
    }
}
