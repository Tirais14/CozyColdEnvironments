using CCEnvs.FuncLanguage;
using System;
using System.Reflection;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Serialization
{
    public abstract class SerializedTypeMember<T> : Serialized<T>
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
