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
        public static ArraySegment<BeforeDisabledGraphicComponentSnapshot> DisableGraphics(
            GameObject input,
            DisableGraphicsSettings settings = DisableGraphicsSettings.Default)
        {
            CC.Guard.IsNotNull(input, nameof(input));

            var results = new List<BeforeDisabledGraphicComponentSnapshot>();

            foreach (var cmp in input.transform.GetComponentsInChildren<Graphic>())
            {
                results.Add(new BeforeDisabledGraphicComponentSnapshot(cmp));

                if (!settings.IsFlagSetted(DisableGraphicsSettings.KeepRaycastTargetState))
                    cmp.raycastTarget = false;

                cmp.color = cmp.color.WithAlpha(0f);
            }

            return results.GetInternalArraySegment();
        }

        public static void EnableGraphics(GameObject input)
        {
            CC.Guard.IsNotNull(input, nameof(input));

            var graphics = input.GetComponentsInChildren<Graphic>();
            for (int i = 0; i < graphics.Length; i++)
                graphics[i].enabled = true;
        }
        public static void EnableGraphics(
            ArraySegment<BeforeDisabledGraphicComponentSnapshot> input)
        {
            CC.Guard.IsNotNull(input, nameof(input));

            for (int i = 0; i < input.Count; i++)
                input[i].Restore();
        }
    }
}
