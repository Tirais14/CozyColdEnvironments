#nullable enable

using UnityEngine;

namespace CCEnvs.Unity.Components.Specialized
{
    public sealed class DontDestroyOnLoad : MonoBehaviour
    {
        public bool SetGameObjectInactive = false;

        private void Awake()
        {
            if (transform.parent == null)
                DontDestroyOnLoad(gameObject);

            if (SetGameObjectInactive)
                gameObject.SetActive(false);

            Destroy(this);
        }
    }
}