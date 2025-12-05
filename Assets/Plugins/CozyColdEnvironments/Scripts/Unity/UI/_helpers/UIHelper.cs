using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public static class UIHelper
    {
        private static readonly Dictionary<Graphic, float> transparentGraphicStateSnapshots = new();

        public static IDisposable DoTransparent(this Graphic graphic)
        {
            CC.Guard.IsNotNull(graphic, nameof(graphic));

            if (transparentGraphicStateSnapshots.ContainsKey(graphic))
                return Disposable.Empty;

            transparentGraphicStateSnapshots.Add(graphic, graphic.color.a);
            graphic.color = graphic.color.WithAlpha(0f);

            return Disposable.CreateWithState(
                graphic,
                graphic => UndoTransparent(graphic)
                );
        }

        public static IDisposable DoTranpsarentRecursive(this Graphic graphic)
        {
            CC.Guard.IsNotNull(graphic, nameof(graphic));

            using var _ = ListPool<IDisposable>.Get(out var cmps);

            foreach (var child in graphic.Q().ByChildren().Components<Graphic>())
                cmps.Add(DoTransparent(child));

            return Disposable.CreateWithState(
                cmps.ToArray(),
                static cmps => cmps.ForEach(x => x.Dispose())
                );
        }

        public static void UndoTransparent(this Graphic graphic)
        {
            if (!transparentGraphicStateSnapshots.Remove(graphic, out float colorAlpha))
                return;

            if (graphic != null)
                graphic.color = graphic.color.WithAlpha(colorAlpha);
        }

        public static void UndoTransparentRecursive(this Graphic graphic)
        {
            CC.Guard.IsNotNull(graphic, nameof(graphic));

            foreach (var cmp in graphic.Q().ByChildren().Components<Graphic>())
                UndoTransparent(cmp);
        }

        public static void CaptureGraphicStatesUntilShowable(GameObject gameObject,
            ICollection<GraphicStateSnaphsot> graphicStates)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            CC.Guard.IsNotNull(graphicStates, nameof(graphicStates));

            var t = gameObject.QueryTo()
                              .ByChildren()
                              .IncludeInactive()
                              .DepthLimiter<IShowable>()
                              .Components<Graphic>()
                              .ToArray();

            foreach (var graphic in gameObject.QueryTo()
                                              .ByChildren()
                                              .IncludeInactive()
                                              .DepthLimiter<IShowable>()
                                              .Components<Graphic>())
            {
                graphicStates!.Add(new GraphicStateSnaphsot(graphic));
            }
        }

        public static void CaptureShowableStatesUntilShowable(GameObject gameObject,
            ICollection<ShowableStateSnapshot> showableStates)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            CC.Guard.IsNotNull(showableStates, nameof(showableStates));

            var t = gameObject.QueryTo()
                              .ByChildren()
                              .IncludeInactive()
                              .ExcludeSelf()
                              .Nearest()
                              .Components<IShowable>()
                              .ToArray();

            _ = t;

            foreach (var showable in gameObject.QueryTo()
                                               .ByChildren()
                                               .IncludeInactive()
                                               .ExcludeSelf()
                                               .Nearest()
                                               .Components<IShowable>())
            {
                showableStates!.Add(new ShowableStateSnapshot(showable));
            }
        }
    }
}
