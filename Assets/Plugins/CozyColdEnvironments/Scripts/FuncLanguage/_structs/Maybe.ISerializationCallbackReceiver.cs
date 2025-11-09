using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEngine.GraphicsBuffer;

namespace CCEnvs.FuncLanguage
{
    public partial struct Maybe : ISerializationCallbackReceiver
    {
#if UNITY_2017_1_OR_NEWER
        readonly void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            IsSome = target.IsNotNull();
        }
#endif
    }
}
