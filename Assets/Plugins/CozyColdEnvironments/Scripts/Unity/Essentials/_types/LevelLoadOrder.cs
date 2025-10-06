using CCEnvs.Unity.EditorSerialization;
using CCEnvs.Unity.Essentials;
using UnityEngine;

namespace CCEnvs.Unity
{
    [CreateAssetMenu(fileName = nameof(LevelLoadOrder), menuName = nameof(CCEnvs) + "/Level Load Order")]
    public class LevelLoadOrder : ScriptableObject
    {
        [field: SerializeField]
        public SerializedImmutableArray<LevelLoadInfo> Order { get; private set; }
    }
}
