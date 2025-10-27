using CCEnvs.FuncLanguage;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public interface IGameObjectBindable
    {
        Maybe<GameObject?> gameObject { get; }
    }
}
