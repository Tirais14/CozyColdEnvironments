using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI
{
    [Serializable]
    public class DisplayItem : IDisplayItem
    {
        [field: SerializeField]
        public Sprite Icon { get; private set; }

        public DisplayItem(Sprite icon)
        {
            Icon = icon;
        }
    }
}
