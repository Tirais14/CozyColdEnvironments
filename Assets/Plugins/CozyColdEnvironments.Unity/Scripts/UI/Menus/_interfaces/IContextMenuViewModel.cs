using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus
{
    public interface IContextMenuViewModel : IViewModel
    {
        IDictionary<string, GameObject> ItemViewPrefabs { get; }
    }
}
