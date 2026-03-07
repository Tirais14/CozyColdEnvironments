using System;
using System.Collections.Generic;
using CCEnvs.Snapshots;
using CCEnvs.Unity.Snapshots.UI;
using R3;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public static class UIHelper
    {
        public static IDisposable DoTransparent(this Graphic graphic)
        {
            CC.Guard.IsNotNull(graphic, nameof(graphic));

            Color color = graphic.color;

            graphic.color = color.WithAlpha(0.01f);

            return Disposable.Create(
                (graphic, color),
                static args =>
                {
                    if (args.graphic == null)
                        return;

                    args.graphic.color = args.color;
                });
        }

        public static void CaptureGraphicStatesUntilShowable(GameObject gameObject,
            ICollection<ISnapshot> graphicStates)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            CC.Guard.IsNotNull(graphicStates, nameof(graphicStates));

            foreach (var graphic in gameObject.QueryTo()
                                              .FromChildrens()
                                              .IncludeInactive()
                                              .DepthLimiter<IShowable>()
                                              .Components<Graphic>())
            {
                graphicStates!.Add(new GraphicSnapshot(graphic));
            }
        }

        public static void CaptureShowableStatesUntilShowable(GameObject gameObject,
            ICollection<ISnapshot> showableStates)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            CC.Guard.IsNotNull(showableStates, nameof(showableStates));

            foreach (var showable in gameObject.QueryTo()
                                               .FromChildrens()
                                               .IncludeInactive()
                                               .ExcludeSelf()
                                               .FirstComponentsOnBranch()
                                               .Components<IShowable>())
            {
                showableStates!.Add(new ShowableSnapshot(showable));
            }
        }
    }
}
