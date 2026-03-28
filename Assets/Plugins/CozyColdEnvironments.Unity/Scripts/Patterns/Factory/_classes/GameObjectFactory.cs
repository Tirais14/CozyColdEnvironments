using CCEnvs.Patterns.Factories;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public class GameObjectFactory : MonoBehaviour, IFactory<GameObject>
    {
        [SerializeField]
        private GameObject prefab;

        public GameObject Create()
        {
            return Instantiate(prefab, transform);
        }
    }
}
