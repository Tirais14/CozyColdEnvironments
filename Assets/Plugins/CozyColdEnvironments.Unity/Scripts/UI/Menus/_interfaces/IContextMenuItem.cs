using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus
{
    public interface IContextMenuItem
    {
        string Name { get; }

        void Invoke();
    }
}
