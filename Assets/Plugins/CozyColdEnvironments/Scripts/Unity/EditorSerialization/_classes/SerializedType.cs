using CCEnvs.Reflection;
using System;

#nullable enable
#pragma warning disable S1117
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public class SerializedType : Serialized<TypeInfo, Type>
    {
        public SerializedType()
        {
        }

        public SerializedType(Type defaultValue) : base(defaultValue)
        {
        }

        protected override Type ConvertToOutput(TypeInfo input)
        {
            return TypeSearch.FindTypeInAppDomain(new TypeSearchArguments
            {
                Namespace = input.Namespace,
                TypeName = input.TypeName,
            });
        }

        protected override TypeInfo ConvertToInput(Type output)
        {
            return new TypeInfo(output.GetName(), output.Namespace);
        }
    }
}
