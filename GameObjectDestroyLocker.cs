using UnityEngine;

#nullable enable

namespace UTIRLib
{
    public sealed class GameObjectDestroyLocker : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Destroy(this);
        }
    }
}