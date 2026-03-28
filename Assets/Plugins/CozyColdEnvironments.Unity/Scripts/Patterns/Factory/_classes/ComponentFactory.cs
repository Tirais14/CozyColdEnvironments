using CCEnvs.Patterns.Factories;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public sealed class ComponentFactory : MonoBehaviour, IFactory<Component>
    {
        [SerializeField]
        private Component prefab;

        public Component Create()
        {
            return Instantiate(prefab, transform);
        }
    }
}
