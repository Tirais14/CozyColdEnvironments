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
                foreach (var child in from go in gameObject.QueryTo().ExcludeSelf().ChildrenGameObjects()
                                      select go.QueryTo().Component<IShowable>().Lax() into cmp
                                      where cmp.IsSome
                                      select cmp.GetValueUnsafe())
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
                        showable.Hide();
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
            foreach (var cmp in gameObject.GetComponents<Graphic>().ZL()
                .Where(graphic =>
                    !graphicSnapshots.Exists(graphicState =>
                        graphicState.Target == graphic))
                )
            {
                graphicSnapshots.Add(new GraphicComponentStateSnapshot(cmp));

                if (!settings.IsFlagSetted(IShowable.Settings.KeepRaycastTargetState))
                    cmp.raycastTarget = false;

                cmp.color = cmp.color.WithAlpha(0f);
            }
        }
    }
}
