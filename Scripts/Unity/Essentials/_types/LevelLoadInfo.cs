using CCEnvs.Unity.EditorSerialization;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
#pragma warning disable S1144
namespace CCEnvs.Unity.Essentials
{
    [Serializable]
    public record LevelLoadInfo
    {
        [field: SerializeField]
        public StringOrNumber SceneKey { get; private set; }

        [field: SerializeField]
        public LoadSceneMode LoadMode { get; private set; } = LoadSceneMode.Single;

        [field: SerializeField]
        public bool SetActive { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Only works on additive loaded scenes")]
        public bool DestroyAfterActivated { get; private set; } = false;
    }
}
