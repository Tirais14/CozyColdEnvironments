using CCEnvs.Linq;
using SuperLinq;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public static class Showable
    {
        public static void Show(GameObject gameObject, ICollection<GraphicStateSnaphsot> hidedComponents)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

            using var d = ListPool<GraphicStateSnaphsot>.Get(out var list);

            foreach (var state in hidedComponents.ZL()
                .Where(state => state.graphic != null))
            {
                state.Restore();
            }

            hidedComponents.Clear();
        }

        public static void Hide(GameObject gameObject, 
            ICollection<GraphicStateSnaphsot> hidedComponents,
            ShowableSettings settings = ShowableSettings.Default)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            CC.Guard.IsNotNull(hidedComponents, nameof(hidedComponents));

            bool hideByColor = settings.IsFlagSetted(ShowableSettings.HideByColor);
            bool keepRaycastTargetState = settings.IsFlagSetted(ShowableSettings.KeepRaycastTargetState);
            bool hideByState = !hideByColor;

            hidedComponents.Clear();

            foreach (var cmp in gameObject.QueryTo()
                                          .ByChildren()
                                          .IncludeInactive()
                                          .SearchDepthLimiter<IShowable>()
                                          .Components<Graphic>()
                                          .ZL()
                                          .Where(cmp => cmp.enabled))
            {
                hidedComponents!.Add(cmp);

                if (hideByState)
                    cmp.enabled = false;
                else
                {
                    if (hideByColor)
                        cmp.color = cmp.color.WithAlpha(0f);

                    if (!keepRaycastTargetState)
                        cmp.raycastTarget = false;
                }
            }
        }
    }
}
