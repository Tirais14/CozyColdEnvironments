using CCEnvs.FuncLanguage;
using System.Reflection;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.EditorSerialization
{
    public abstract class SerializedTypeMember<T> : EditorSerialized<T>
        where T : MemberInfo
    {
        [SerializeField]
        protected SerializedType declaringType = null!;

        [SerializeField]
        protected Maybe<string> memberName;

        [SerializeField]
        protected BindingFlags bindingFlags;

        protected SerializedTypeMember()
        {
        }

        protected SerializedTypeMember(T defaultValue) : base(defaultValue)
        {
        }
    }
}
