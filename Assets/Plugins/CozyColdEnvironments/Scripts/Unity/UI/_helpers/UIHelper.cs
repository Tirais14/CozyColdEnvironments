using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Timers;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using ZLinq;
using static UnityEngine.Experimental.Rendering.GraphicsStateCollection;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public static class UIHelper
    {
        private static readonly Dictionary<Graphic, float> transparentGraphicStateSnapshots = new();

        public static IDisposable DoTransparent(Graphic graphic)
        {
            CC.Guard.IsNotNull(graphic, nameof(graphic));

            if (transparentGraphicStateSnapshots.ContainsKey(graphic))
                return Disposable.Empty;

            transparentGraphicStateSnapshots.Add(graphic, graphic.color.a);
            graphic.color = graphic.color.WithAlpha(0f);

            return Disposable.CreateWithState(
                graphic,
                graphic => transparentGraphicStateSnapshots.Remove(graphic)
                );
        }

        public static void UndoTransparent(Graphic graphic)
        {
            if (!transparentGraphicStateSnapshots.Remove(graphic, out float colorAlpha))
                return;

            if (graphic != null)
                graphic.color = graphic.color.WithAlpha(colorAlpha);
        }

        public static void CaptureGraphicStates(GameObject gameObject,
            ICollection<GraphicStateSnaphsot> graphicStates)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            CC.Guard.IsNotNull(graphicStates, nameof(graphicStates));

            graphicStates.Clear();
            foreach (var graphic in gameObject.QueryTo()
                                              .ByChildren()
                                              .IncludeInactive()
                                              .DepthLimiter<IShowable>()
                                              .Components<Graphic>())
            {
                graphicStates!.Add(graphic);
            }
        }

        public static void CaptureShowableStates(GameObject gameObject,
            ICollection<ShowableStateSnapshot> showableStates)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            CC.Guard.IsNotNull(showableStates, nameof(showableStates));

            showableStates.Clear();
            foreach (var showable in gameObject.QueryTo()
                                               .ByChildren()
                                               .IncludeInactive()
                                               .ExcludeSelf()
                                               .Components<IShowable>())
            {
                showableStates!.Add(new ShowableStateSnapshot(showable));
            }
        }
    }
}
