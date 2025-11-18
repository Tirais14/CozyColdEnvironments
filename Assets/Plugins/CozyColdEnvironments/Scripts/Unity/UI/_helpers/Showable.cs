using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using SuperLinq;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public static class Showable
    {
        public static void Show(GameObject gameObject,
            List<GraphicComponentStateSnapshot> graphicSnapshots,
            IShowable.Settings settings = IShowable.Settings.Default)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            Guard.IsNotNull(graphicSnapshots, nameof(graphicSnapshots));

            if (graphicSnapshots.IsEmpty())
                return;

            foreach (var cmpShapshot in graphicSnapshots)
                cmpShapshot.Restore();

            if (settings.IsFlagSetted(IShowable.Settings.Recursive))
            {
                foreach (var child in gameObject.QueryTo()
                                                .ByChildren()
                                                .ExcludeSelf()
                                                .Models<IShowable>())
                {
                    child.Show(settings & ~IShowable.Settings.Recursive);
                }
            }
        }

        public static void Hide(GameObject gameObject,
            List<GraphicComponentStateSnapshot> graphicSnapshots,
            IShowable.Settings settings = IShowable.Settings.Default)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            Guard.IsNotNull(graphicSnapshots, nameof(graphicSnapshots));

            graphicSnapshots.Clear();

            HideGraphics(gameObject, graphicSnapshots, settings);

            if (settings.IsFlagSetted(IShowable.Settings.Recursive))
            {
                foreach (var child in gameObject.QueryTo().ChildrenGameObjects())
                {
                    if (child.QueryTo()
                             .Component<IShowable>()
                             .Lax()
                             .TryGetValue(out var showable)
                       &&
                       showable.IsVisible)
                    {
                        showable.Hide(settings & ~IShowable.Settings.Recursive);
                    }
                    else
                        HideGraphics(child, graphicSnapshots, settings);
                }
            }
        }

        private static void HideGraphics(GameObject gameObject,
            List<GraphicComponentStateSnapshot> graphicSnapshots, 
            IShowable.Settings settings)
        {
            if (graphicSnapshots.Exists(x => x.Target.gameObject == gameObject))
                return;

            Graphic[] snapshotTargets = graphicSnapshots.Select(cmp => cmp.Target).ToArray();

            foreach (var cmp in gameObject.QueryTo()
                .Components<Graphic>()
                .ZL()
                .Where(cmp => cmp.enabled)
                .Where(cmp => !snapshotTargets.Contains(cmp)))
            {
                graphicSnapshots.Add(new GraphicComponentStateSnapshot(cmp));

                if (settings.IsFlagSetted(IShowable.Settings.ByComponentState))
                    cmp.enabled = false;
                else
                {
                    if (!settings.IsFlagSetted(IShowable.Settings.KeepRaycastTargetState))
                        cmp.raycastTarget = false;

                    cmp.color = cmp.color.WithAlpha(0f);
                }
            }
        }
    }
}
