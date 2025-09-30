using ZLinq;
using System.Linq;
using UnityEngine;

namespace CCEnvs.Unity.Components.Specialized
{
    public class DisableOnStart : MonoBehaviour
    {
        private MonoBehaviour[] monos;

        private void Awake()
        {
            monos = GetComponents<MonoBehaviour>().Where(x => x.enabled).ToArray();
        }

        private void Update()
        {
            if (monos.All(x => x.didStart))
                gameObject.SetActive(false);
        }
    }
}
