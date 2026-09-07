using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus
{
    public interface IContextMenuViewModel : IViewModel
    {
        event Action<IContextMenuItem> OnItemInvoke;

        GameObject? DefaultItemViewPrefab { get; set; }

        IDictionary<string, GameObject> ItemViewPrefabs { get; }
    }
}
