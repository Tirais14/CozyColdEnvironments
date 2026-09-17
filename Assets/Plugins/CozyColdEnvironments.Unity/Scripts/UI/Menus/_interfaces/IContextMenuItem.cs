using System;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus
{
    public interface IContextMenuItem
    {
        event Action OnInvoke;

        string Name { get; }

        void Invoke();
    }
}
