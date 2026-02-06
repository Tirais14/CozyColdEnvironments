using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    [DisallowMultipleComponent]
    public class KeyMarker<T> : MonoBehaviour
    {
        public T Key;
    }
}
