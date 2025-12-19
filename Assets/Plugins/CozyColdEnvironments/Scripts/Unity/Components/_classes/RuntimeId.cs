using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Components
{
    [Serializable]
    [DisallowMultipleComponent]
    public sealed class RuntimeId : CCBehaviour
    {
        [Obsolete]
        public const string DEFAULT_ID_VALUE = "Undefined";

        [field: SerializeField]
        public string? Id { get; private set; }
    }
}
