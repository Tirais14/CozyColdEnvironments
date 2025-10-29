using CCEnvs.Collections.Performance;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public static class UIHelper
    {
        public static ArraySegment<BeforeDisabledGraphicComponentSnapshot> DisableGraphics(Component component)
        {
            CC.Guard.IsNotNull(component, nameof(component));

            var results = new List<BeforeDisabledGraphicComponentSnapshot>();

            var graphics = component.transform.GetComponentsInChildren<Graphic>();
            for (int i = 0; i < graphics.Length; i++)
                results.Add(new BeforeDisabledGraphicComponentSnapshot(graphics[i]));

            return results.GetInternalArraySegment();
        }

        public static void EnableGraphics(Component component)
        {
            CC.Guard.IsNotNull(component, nameof(component));

            var graphics = component.GetComponentsInChildren<Graphic>();
            for (int i = 0; i < graphics.Length; i++)
                graphics[i].enabled = true;
        }
        public static void EnableGraphics(ArraySegment<BeforeDisabledGraphicComponentSnapshot> settings)
        {
            CC.Guard.IsNotNull(settings, nameof(settings));

            for (int i = 0; i < settings.Count; i++)
                settings[i].Apply();
        }
    }
}
