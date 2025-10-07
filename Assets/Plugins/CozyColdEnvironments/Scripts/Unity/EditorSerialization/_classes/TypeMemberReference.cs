using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public abstract class TypeMemberReference<T>
        : Serialized<SerializedTuple<SerializedType, string, MemberBindings>, T>
        where T : MemberInfo
    {
        protected override bool EraseInputOnSerialized => false;

        protected TypeMemberReference()
        {
        }

        protected TypeMemberReference(T defaultValue)
            :
            base(defaultValue)
        {
        }

        protected override SerializedTuple<SerializedType, string, MemberBindings> ConvertToInput(T output) => input;
    }
}
