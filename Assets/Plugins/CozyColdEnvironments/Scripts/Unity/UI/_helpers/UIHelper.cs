using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CCEnvs.FuncLanguage;
using CCEnvs.Collections.Unsafe;
using CCEnvs.Collections.Performance;
using System;
using SuperLinq;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public static class UIHelper
    {
        public static ArraySegment<BeforeDisabledGraphicSnapshot> DisableGraphics(Component component)
        {
            CC.Guard.IsNotNull(component, nameof(component));

            var results = new List<BeforeDisabledGraphicSnapshot>();

            var graphics = component.transform.GetComponentsInChildren<MaskableGraphic>();
            for (int i = 0; i < graphics.Length; i++)
                results.Add(new BeforeDisabledGraphicSnapshot(graphics[i]));

            return results.GetInternalArraySegment();
        }

        public static void EnableGraphics(Component component)
        {
            CC.Guard.IsNotNull(component, nameof(component));

            var graphics = component.GetComponentsInChildren<MaskableGraphic>();
            for (int i = 0; i < graphics.Length; i++)
                graphics[i].enabled = true;
        }
        public static void EnableGraphics(ArraySegment<BeforeDisabledGraphicSnapshot> settings)
        {
            CC.Guard.IsNotNull(settings, nameof(settings));

            for (int i = 0; i < settings.Count; i++)
                settings[i].Apply();
        }
    }
}
