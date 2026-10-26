using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

#nullable enable
namespace CCEnvs.Json
{
    public class DescriptedObjectJsonConverter : JsonConverter
    {
        public const string DESCRIPTOR_PROPERTY_NAME = "$typeDescriptor";

        public override bool CanConvert(Type objectType)
        {
            if (TypeSerializationHelper.TypeDescriptors.ContainsKey(objectType))
                return true;

            if (objectType.IsDefined<PolymorphSerializableAttribute>(inherit: true)
                || objectType == TypeofCache<object>.Type
                || objectType.IsAbstract
                || objectType.IsInterface)
            {
                return true;
            }

            return false;
        }

        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jToken = JToken.Load(reader);

            // 1. Если это не JObject или в нем вообще нет свойства дескриптора
            if (jToken is not JObject jObj || !jObj.Properties().Any(prop => prop.Name == DESCRIPTOR_PROPERTY_NAME))
            {
                return FallbackDeserialize(jToken, objectType, serializer);
            }

            // 2. Если свойство есть, но его значение null
            if (jObj.Property(DESCRIPTOR_PROPERTY_NAME) is not { } descriptorProp || descriptorProp.Value.Type == JTokenType.Null)
            {
                return FallbackDeserialize(jToken, objectType, serializer);
            }

            // 3. Пытаемся прочитать дескриптор
            var descriptor = descriptorProp.Value.ToObject<TypeSerializationDescriptor>(serializer);

            // 4. Если дескриптор невалиден или тип не найден в реестре
            if (descriptor == default || !TypeSerializationHelper.DescriptedTypes.TryGetValue(descriptor, out var resultType))
            {
                return FallbackDeserialize(jToken, objectType, serializer);
            }

            // ✅ Успешный путь: дескриптор найден, удаляем его и читаем объект
            jObj.Remove(DESCRIPTOR_PROPERTY_NAME);
            return OnReadJson(jObj, resultType, serializer);
        }

        /// <summary>
        /// Попытка десериализовать объект другими конвертерами, исключая текущий, чтобы избежать бесконечной рекурсии.
        /// </summary>
        private object? FallbackDeserialize(JToken jToken, Type objectType, JsonSerializer originalSerializer)
        {
            if (CCDebug.Instance.IsEnabled)
            {
                this.PrintLog($"Descriptor missing or invalid for type '{objectType}'. Falling back to other converters.");
            }

            // Создаем настройки, исключая текущий конвертер (используем ваш существующий метод расширения)
            var fallbackSettings = originalSerializer.GetSerializerSettings().ExcludeConverters(this);
            var fallbackSerializer = JsonSerializer.Create(fallbackSettings);

            // Копируем важный контекст из оригинального сериализатора
            fallbackSerializer.Context = originalSerializer.Context;
            fallbackSerializer.Culture = originalSerializer.Culture;
            fallbackSerializer.Formatting = originalSerializer.Formatting;

            // Пытаемся десериализовать через остальные конвертеры
            var result = fallbackSerializer.Deserialize(jToken.CreateReader(), objectType);

            // Защита: если результат все еще JToken, значит ни один другой конвертер не смог обработать этот тип 
            // (часто бывает с абстрактными классами/интерфейсами без подходящего конвертера)
            if (result is JToken && objectType.IsNotType<JToken>())
            {
                throw new JsonSerializationException(
                    $"Cannot deserialize type '{objectType}'. No suitable converter found after excluding DescriptedObjectJsonConverter.");
            }

            return result;
        }

        public override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer serializer)
        {
            if (value.IsNull())
            {
                writer.WriteNull();
                return;
            }

            var objType = value.GetType();

            if (!TypeSerializationHelper.TryGetDescriptor(objType, out var descriptor))
                throw new JsonSerializationException($"Type: {value.GetType()} hasn't type descriptor");

            // Сохраняем оригинальное значение, чтобы восстановить его после записи
            var originalTypeNameHandling = serializer.TypeNameHandling;
            serializer.TypeNameHandling = TypeNameHandling.None;

            var contract = serializer.ContractResolver.ResolveContract(objType);

            if (contract is JsonObjectContract jObjContract)
            {
                writer.WriteStartObject();

                writer.WritePropertyName(DESCRIPTOR_PROPERTY_NAME);
                serializer.Serialize(writer, descriptor);

                JsonSerializerInternalWriterHelper.WriteObjectBody(
                    writer,
                    jObjContract,
                    serializer,
                    value);

                writer.WriteEndObject();
            }
            else
            {
                writer.WriteStartObject();
                writer.WritePropertyName(DESCRIPTOR_PROPERTY_NAME);
                serializer.Serialize(writer, descriptor);
                writer.WritePropertyName("value");

                var tSerializer = JsonSerializer.Create(serializer.GetSerializerSettings().ExcludeConverters(this));
                tSerializer.Serialize(writer, value);
                writer.WriteEndObject();
            }

            // Восстанавливаем настройку
            serializer.TypeNameHandling = originalTypeNameHandling;
        }

        // Оставляем виртуальные методы для возможности переопределения в наследниках, 
        // но по умолчанию они больше не выбрасывают исключения, а делегируют работу FallbackDeserialize.
        // Если вам нужно строгое поведение в конкретных наследниках, переопределите их там.
        protected virtual object? OnDescriptorPropertyNotFound(JObject jObj, Type objType, JsonSerializer serializer)
        {
            return FallbackDeserialize(jObj, objType, serializer);
        }

        protected virtual object? OnDescriptorNotFound(JObject jObj, Type objType, JsonSerializer serializer)
        {
            return FallbackDeserialize(jObj, objType, serializer);
        }

        protected virtual object? OnReadJson(JObject jObj, Type objType, JsonSerializer serializer)
        {
            var instance = JsonSerializerInternalReaderHelper.CreateNewObject(objType, jObj.CreateReader(), serializer);
            serializer.Populate(jObj.CreateReader(), instance);

            return instance;
        }
    }
}