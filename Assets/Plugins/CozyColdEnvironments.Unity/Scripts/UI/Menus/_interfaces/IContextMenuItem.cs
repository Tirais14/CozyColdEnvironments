using System;
using UnityEngine;

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
