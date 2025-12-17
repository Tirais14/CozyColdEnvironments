using CCEnvs.Snapshots;
using CCEnvs.Unity.Snapshots.UI;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using R3;
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

            return Disposable.Create(
                graphic,
                graphic => UndoTransparent(graphic)
                );
        }

        public static IDisposable DoTranpsarentRecursive(this Graphic graphic)
        {
            CC.Guard.IsNotNull(graphic, nameof(graphic));

            using var _ = ListPool<IDisposable>.Get(out var cmps);

            foreach (var child in graphic.Q().FromChildrens().Components<Graphic>())
                cmps.Add(DoTransparent(child));

            return Disposable.Create(
                cmps.ToArray(),
                static cmps => cmps.ForEach(x => x.Dispose())
                );
        }

        public static IDisposable DoTranpsarentRecursive(GameObject gameObject)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

            return gameObject.Q()
                .Component<Graphic>()
                .Lax()
                .Map(x => x.DoTranpsarentRecursive())
                .GetValue(() => Disposable.Empty);
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

            foreach (var cmp in graphic.Q().FromChildrens().Components<Graphic>())
                UndoTransparent(cmp);
        }

        public static void UndoTransparentRecursive(GameObject gameObject)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

            gameObject.Q()
                .Component<Graphic>()
                .Lax()
                .IfSome(x => x.UndoTransparentRecursive());
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
