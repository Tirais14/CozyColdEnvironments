using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using CCEnvs.Caching;
using CCEnvs.Collections;
using CCEnvs.Pools;
using CCEnvs.Reflection;
using Humanizer;
using Newtonsoft.Json;
using ObservableCollections;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public class ObservableDictionaryJsonConverter
        :
        JsonConverter
    {
        private readonly static Cache<(Type Type, bool WithParameter), ConstructorInfo> ctors = new()
        {
            ExpirationScanFrequency = 30.Minutes()
        };

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType
                   &&
                   objectType.GetGenericTypeDefinition() == typeof(ObservableDictionary<,>);
        }

        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
            )
        {
            if (reader.TokenType != JsonToken.Null)
                return null;

            using var items = ListPool<object>.Shared.Get();

            object? item;

            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
            {
                try
                {
                    item = serializer.Deserialize<object>(reader);

                    if (item is null)
                        continue;

                    items.Value.Add(item);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                    continue;
                }
            }

            ConstructorInfo? ctor;

            object dictionary;

            if (items.Value.IsEmpty())
            {
                if (!ctors.TryGetValue((objectType, WithParameter: false), out ctor))
                {
                    ctor = objectType.GetConstructor(
                        BindingFlagsDefault.InstancePublic,
                        null,
                        Type.EmptyTypes,
                        new arr<ParameterModifier>()
                        );

                    if (ctors.TryAdd((objectType, WithParameter: false), ctor, out var entry))
                        entry.ExpirationTimeRelativeToNow = 10.Minutes();
                }

                dictionary = ctor.Invoke(CC.EmptyArguments);

                return dictionary;
            }

            if (!ctors.TryGetValue((objectType, WithParameter: true), out ctor))
            {
                ctor = objectType.GetConstructors(BindingFlagsDefault.InstancePublic)
                    .Select(ctor => (ctor, prms: ctor.GetParameters()))
                    .Where(ctorInfo => ctorInfo.prms.Length == 1)
                    .FirstOrDefault(ctorInfo => ctorInfo.prms[0].ParameterType.IsType<IEnumerable>())
                    .ctor;

                if (ctors.TryAdd((objectType, WithParameter: true), ctor, out var entry))
                    entry.ExpirationTimeRelativeToNow = 10.Minutes();
            }

            dictionary = ctor.Invoke(new object[] { items.Value });

            return dictionary;
        }

        public override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer serializer
            )
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartArray();

            foreach (var item in value.To<IEnumerable>())
                serializer.Serialize(writer, item);

            writer.WriteEndArray();
        }
    }
}
