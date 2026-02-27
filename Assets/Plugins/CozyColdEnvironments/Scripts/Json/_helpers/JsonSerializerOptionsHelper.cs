using System;
using System.Linq;
using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;

#nullable enable
namespace CCEnvs.Json
{
    public static class JsonSerializerOptionsHelper
    {
        public static JsonSerializerSettings ExcludeConverters(
            this JsonSerializerSettings source,
            params Type[] converterTypes)
        {
            Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(converterTypes, nameof(converterTypes));

            if (source.Converters.IsReadOnly)
            {
                var options = new JsonSerializerSettings(source);
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

        public static JsonSerializerSettings ExcludeConverters(
            this JsonSerializerSettings source,
            params JsonConverter[] converters)
        {
            Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(converters, nameof(converters));

            return source.ExcludeConverters(converters.Select(x => x.GetType()).ToArray());
        }

        public static JsonSerializerSettings AddConverters(
            this JsonSerializerSettings source,
            params JsonConverter[] converters)
        {
            Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(converters, nameof(converters));

            if (source.Converters.IsReadOnly)
            {
                var options = new JsonSerializerSettings(source);
                foreach (var conv in converters)
                    options.Converters.Add(conv);

                return options;
            }

            foreach (var conv in converters)
                source.Converters.Add(conv);

            return source;
        }

        public static JsonSerializerSettings Clone(this JsonSerializerSettings source)
        {
            Guard.IsNotNull(source, nameof(source));
            return new JsonSerializerSettings(source);
        }
    }
}
