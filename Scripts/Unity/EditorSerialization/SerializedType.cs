using CCEnvs.Reflection;
using System;

#nullable enable
#pragma warning disable S1117
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public class SerializedType : Serialized<TypeInfo, Type>
    {
        protected override Type GetOutput()
        {
            return TypeSearch.FindTypeInAppDomain(new TypeFinderParameters
            {
                Namespace = input.Namespace,
                TypeName = input.TypeName,
            });
        }

        protected override TypeInfo GetInput()
        {
            return new TypeInfo(Output.GetName(), Output.Namespace);
        }
    }
}
