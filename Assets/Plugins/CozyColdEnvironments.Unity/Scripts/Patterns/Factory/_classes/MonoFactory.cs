using CCEnvs.Patterns.Factories;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public abstract class MonoFactory : MonoBehaviour, IFactory
    {
        public abstract object Create(params object[] args);
    }
}
