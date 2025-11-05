#nullable enable
using CCEnvs.Conversations;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Runtime.Serialization;

#pragma warning disable S2743
#pragma warning disable S2696
namespace CCEnvs.Json.Converters
{
    public static class PolymorphJsonConverter
    {
        public delegate Type TypeResolver(Type polymorphType, object criteria);
    }

    /// <summary>
    /// Only converts specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PolymorphJsonConverter<T> : CCJsonConverter<T>
    {

        private static int readInvokesCount;
        private static int writeInvokesCount;

        private readonly Type[] types;

        public PolymorphJsonConverter(params Type[] types)
        {
            this.types = types;
        }

        private static Type GetConversationType(JsonSerializer serializer, JToken token)
        {

            NamingStrategy namingStrategy = serializer.ContractResolver.ReflectQuery()
                .NonPublic()
                .ExtraType<NamingStrategy>()
                .Field()
                .Strict()
                .GetValue(serializer.ContractResolver)
                .As<NamingStrategy>();

            string keyName = namingStrategy.GetPropertyName(nameof(ITypeProvider.ObjectType), false);
            JToken? objectTypeToken = token[keyName];

            switch (serializer.Context.Context)
            {
                case Type type:
                    return type;
                case PolymorphConverterContext context:
                    return context.DeserializedType;
                default:
                    break;
            }

            if (objectTypeToken is null)
                throw new JsonSerializationException($"{typeof(PolymorphJsonConverter<T>).GetFullName()} must contain any variant of resolving type method, such as: {nameof(Type)} in {nameof(StreamingContext)} or {nameof(PolymorphConverterContext)}");

            return objectTypeToken.ToObject<Type>(serializer)
                   ??
                   CC.Throw.InvalidCast(typeof(JToken)).As<Type>();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object? ReadJson(JsonReader reader,
                                         Type objectType,
                                         object? existingValue,
                                         JsonSerializer serializer)
        {
            readInvokesCount++;

            if (readInvokesCount > 3)
                throw new JsonSerializationException("Prevented dead loop in polymorph converter.");

            try
            {
                PrepareSerializer(ref reader, ref serializer, out JToken token);

                Type conversationType = GetConversationType(serializer, token);
                object? deserialized = DeserializeObject(conversationType, token, serializer);

                if (deserialized is null)
                    return null;

                return TypeMutator.MutateType(deserialized, typeof(T));
            }
            finally
            {
                readInvokesCount--;
            }
        }

        public override void WriteJson(JsonWriter writer,
                                       object? value,
                                       JsonSerializer serializer)
        {
            if (value.IsNull())
            {
                writer.WriteNull();
                return;
            }

            writeInvokesCount++;

            if (writeInvokesCount > 3)
                throw new JsonSerializationException("Prevented dead loop in polymorph converter.");

            try
            {
                

                var obj = ((ITypeProvider)value).ObjectType.ReflectQuery()
                    .NonPublic()
                    .Cache()
                    .Arguments(value)
                    .Invoke().Raw;

                serializer.Serialize(writer, obj);
            }
            finally
            {
                writeInvokesCount--;
            }
        }
    }
}
