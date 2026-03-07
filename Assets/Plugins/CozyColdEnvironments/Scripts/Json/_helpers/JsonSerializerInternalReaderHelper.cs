using System;
using System.Linq;
using System.Reflection;
using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;

#nullable enable
namespace CCEnvs.Json
{
    public class JsonSerializerInternalReaderHelper
    {
        private readonly static Lazy<Type> readerType = new(
            static () =>
            {
                return Type.GetType($"Newtonsoft.Json.Serialization.JsonSerializerInternalReader, Newtonsoft.Json", throwOnError: true);
            });

        private static ConstructorInfo? readerCtor;

        private static MethodInfo? createNewObjectMethod;

        public static Type ReaderType => readerType.Value;

        public static object CreateNewObject(
            Type type,
            JsonReader reader,
            JsonSerializer serializer
            )
        {
            Guard.IsNotNull(type, nameof(type));
            Guard.IsNotNull(reader, nameof(reader));
            Guard.IsNotNull(serializer, nameof(serializer));

            var contract = serializer.ContractResolver.ResolveContract(type);

            var intReader = CreateReader(serializer);

            var createObjectMethod = GetCreateNewObjectMethod();

            var prms = new object?[] //CreateObject
            {
                reader,
                contract,
                null,
                null,
                null,
                null,
            };

            //Magic offset :>
            reader.Read();
            reader.Read();

            var instance = createObjectMethod.Invoke(intReader, prms);

            return instance;
        }

        public static object CreateReader(JsonSerializer serializer)
        {
            if (readerCtor is null)
            {
                readerCtor = readerType.Value.GetConstructor(
                    BindingFlagsDefault.InstancePublic,
                    binder: null,
                    Range.From(typeof(JsonSerializer)),
                    new arr<ParameterModifier>()
                    );
            }

            return readerCtor.Invoke(new object[] { serializer });
        }

        private static MethodInfo GetCreateNewObjectMethod()
        {
            if (createNewObjectMethod is null)
            {
                var methods = readerType.Value.GetMethods(BindingFlagsDefault.InstancePublic | BindingFlags.DeclaredOnly);

                createNewObjectMethod = methods.FirstOrDefault(static method => method.Name == "CreateNewObject")
                    ??
                    throw new InvalidOperationException("Cannot find method: CreateNewObject");
            }

            return createNewObjectMethod;
        }
    }
}
