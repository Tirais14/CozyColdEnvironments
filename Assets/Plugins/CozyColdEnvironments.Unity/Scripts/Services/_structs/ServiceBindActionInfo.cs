using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Services
{
    [Serializable]
    public struct ServiceBindActionInfo
    {
        [field: SerializeField]
        public Component Component { get; private set; }

        [field: SerializeField]
        public string? ID { get; private set; }

        [field: SerializeField]
        public ServiceBindActionOptions Options { get; private set; }

        [field: SerializeField]
        public string? InterfacesFilter { get; private set; }
    }
}
