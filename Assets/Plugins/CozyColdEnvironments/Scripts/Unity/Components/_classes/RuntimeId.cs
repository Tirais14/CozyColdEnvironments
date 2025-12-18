using CommunityToolkit.Diagnostics;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Components
{
    [Serializable]
    [DisallowMultipleComponent]
    public sealed class RuntimeId : CCBehaviour
    {
        [field: SerializeField]
        public string Id { get; private set; } = "Undefined";

        public void SetId(string id)
        {
            Guard.IsNotNull(id);
            Id = id;
        }
    }
}
