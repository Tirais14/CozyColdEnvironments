using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public interface IFindComponentQuery<out TThis>
    {
        GameObject Source { get; }
        bool includeInactive { get; }
        FindMode findMode { get; }

        TThis From(GameObject gameObject);

        TThis From(Component component);

        TThis IncludeInactive();

        TThis InSelf();

        TThis InChildren();

        TThis InParent();

        TThis Reset();
    }
}
