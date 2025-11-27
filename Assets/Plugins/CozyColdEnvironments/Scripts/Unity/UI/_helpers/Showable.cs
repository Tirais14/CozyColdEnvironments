using CCEnvs.Linq;
using SuperLinq;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public static class Showable
    {
        public static void Show(GameObject gameObject,
            ICollection<GraphicStateSnaphsot> graphicStates,
            ICollection<ShowableStateSnapshot> showableStates)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

            foreach (var state in graphicStates.ZL()
                .Where(state => state.Target != null))
            {
                state.Restore();
            }
            graphicStates.Clear();

            foreach (var state in showableStates)
            {
                if (state.gameObject.Raw != gameObject)
                    state.Restore();
            }
            showableStates.Clear();
        }

        public static void Hide(GameObject gameObject,
            ICollection<GraphicStateSnaphsot> graphicStates,
            ICollection<ShowableStateSnapshot> showableStates,
            ShowableSettings settings = ShowableSettings.Default)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            CC.Guard.IsNotNull(graphicStates, nameof(graphicStates));

            bool hideByColor = settings.IsFlagSetted(ShowableSettings.HideByColor);
            bool keepRaycastTargetState = settings.IsFlagSetted(ShowableSettings.KeepRaycastTargetState);
            bool hideByState = !hideByColor;

            graphicStates.Clear();
            
            foreach (var graphic in gameObject.QueryTo()
                                              .ByChildren()
                                              .IncludeInactive()
                                              .DepthLimiter<IShowable>()
                                              .Components<Graphic>()
                                              .ZL()
                                              .Where(cmp => cmp.enabled))
            {
                graphicStates!.Add(graphic);

                if (hideByState)
                    graphic.enabled = false;
                else
                {
                    if (hideByColor)
                        graphic.color = graphic.color.WithAlpha(0f);

                    if (!keepRaycastTargetState)
                        graphic.raycastTarget = false;
                }
            }

            foreach (var showable in gameObject.Q()
                                               .ByChildren()
                                               .IncludeInactive()
                                               .ExcludeSelf()
                                               .Components<IShowable>())
            {
                showableStates.Add(new ShowableStateSnapshot(showable));
                showable.Hide();
            }
        }
    }
}
