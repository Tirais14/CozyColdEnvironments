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
            :
            base(FindTypeByName, output => new TypeInfo(output.Name, output.GetName()))
        {
        }

        public static implicit operator Type?(SerializedType value)
        {
            return value.Output;
        }

        private static Type FindTypeByName(TypeInfo input)
        {
            return TypeSearch.FindTypeInAppDomain(new TypeFinderParameters
            {
                Namespace = input.Namespace,
                TypeName = input.TypeName,
            });
        }
    }
}
