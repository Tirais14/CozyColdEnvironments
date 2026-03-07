using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CCEnvs.Collections;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#nullable enable
namespace CCEnvs.Json
{
    public class JsonSerializerInternalWriterHelper
    {
        private readonly static Lazy<Type> jsonInternalWriterType = new(
        static () =>
        {
            return Type.GetType($"Newtonsoft.Json.Serialization.JsonSerializerInternalWriter, Newtonsoft.Json", throwOnError: true);
        });

        private static ConstructorInfo? jsonInternalWriterCtor;

        private static MethodInfo? calculatePropertyValuesMethod;

        public static void WriteObjectBody(
            JsonWriter writer,
            JsonObjectContract contract,
            JsonSerializer serializer,
            object target
            )
        {
            foreach (var jProp in contract.Properties)
            {
                if (!CalculatePropertyValues(
                    serializer,
                    writer,
                    target,
                    contract,
                    null,
                    jProp,
                    out _,
                    out var memberValue
                    ))
                {
                    continue;
                }

                if (writer.WriteState == WriteState.Property)
                    writer.WriteNull();

                writer.WritePropertyName(jProp.PropertyName!);

                serializer.Serialize(writer, memberValue);
            }
        }

        public static bool CalculatePropertyValues(
            JsonSerializer serializer,
            JsonWriter writer,
            object value,
            JsonContainerContract contract,
            JsonProperty? member,
            JsonProperty property,
            out JsonContract? memberContract,
            out object? memberValue
            )
        {
            Guard.IsNotNull(serializer, nameof(serializer));
            Guard.IsNotNull(writer, nameof(writer));
            CC.Guard.IsNotNull(value, nameof(value));
            Guard.IsNotNull(contract, nameof(contract));
            Guard.IsNotNull(property, nameof(property));

            var prms = new object?[]
            {
                writer,
                value,
                contract,
                member,
                property,
                null,
                null,
            };

            var method = GetCalculatePropertyValuesMethod();

            var internalWriter = CreateJsonInternalWriter(serializer);

            bool result = (bool)method.Invoke(internalWriter, prms);

            memberContract = (JsonContract?)prms[5];
            memberValue = prms[6];

            return result;
        }

        private static object CreateJsonInternalWriter(JsonSerializer serializer)
        {
            if (jsonInternalWriterCtor is null)
            {
                jsonInternalWriterCtor = jsonInternalWriterType.Value.GetConstructor(
                    BindingFlagsDefault.InstancePublic,
                    binder: null,
                    Range.From(TypeofCache<JsonSerializer>.Type),
                    new arr<ParameterModifier>()
                    )
                    ??
                    throw new InvalidOperationException($"Cannot found constructor of type: {jsonInternalWriterType.Value}");
            }

            return jsonInternalWriterCtor.Invoke(new object[] { serializer });
        }

        private static MethodInfo GetCalculatePropertyValuesMethod()
        {
            if (calculatePropertyValuesMethod is null)
            {
                calculatePropertyValuesMethod = jsonInternalWriterType.Value.GetMethods(BindingFlagsDefault.InstanceAll)
                    .FirstOrDefault(static method => method.Name == "CalculatePropertyValues")
                    ??
                    throw new InvalidOperationException("Cannot find method: CalculatePropertyValues");
            }

            return calculatePropertyValuesMethod;
        }
    }
}
