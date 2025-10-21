using CCEnvs.Language;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public interface IGameObjectBindable
    {
        Ghost<GameObject?> gameObject { get; }
    }
}
