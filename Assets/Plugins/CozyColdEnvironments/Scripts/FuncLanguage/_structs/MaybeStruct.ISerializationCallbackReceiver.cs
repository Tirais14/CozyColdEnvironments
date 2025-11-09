#if UNITY_2017_1_OR_NEWER
using UnityEngine;
using static CCEnvs.FuncLanguage.LangOperator;

#nullable enable
namespace CCEnvs.FuncLanguage
{
    public partial struct MaybeStruct<T> : ISerializationCallbackReceiver
        where T : struct
    {
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            IsSome = IsSome(target, @default);
        }

        readonly void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }
    }
}
#endif