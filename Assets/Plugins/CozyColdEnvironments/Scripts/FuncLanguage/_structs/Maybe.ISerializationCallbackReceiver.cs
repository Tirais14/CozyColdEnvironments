#if UNITY_2017_1_OR_NEWER
using UnityEngine;
using static CCEnvs.FuncLanguage.LangOperator;

namespace CCEnvs.FuncLanguage
{
    public partial struct Maybe<T> : ISerializationCallbackReceiver
    {
        readonly void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            IsSome = IsSome(target, @default);
        }
    }
}
#endif
