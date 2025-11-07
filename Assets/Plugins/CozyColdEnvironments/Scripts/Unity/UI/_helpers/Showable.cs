using SuperLinq;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public static class Showable
    {
        public static void Show(GameObject gameObject,
            List<GraphicComponentStateSnapshot> componentSnapshots,
            IShowable.Settings settings = IShowable.Settings.Default)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            CC.Guard.IsNotNull(componentSnapshots, nameof(componentSnapshots));

            if (componentSnapshots.IsEmpty())
                return;

            foreach (var cmpShapshot in componentSnapshots)
                cmpShapshot.Restore();

            if (settings.IsFlagSetted(IShowable.Settings.Recursive))
            {
                foreach (var cmp in gameObject.FindFor()
                    .InChildren()
                    .Transforms()
                    .Select(child => child.AsOrDefault<IShowable>())
                    .Where(showable => showable.IsSome)
                    .Select(showable => showable.AccessUnsafe()))
                {
                    cmp.Show(settings & ~IShowable.Settings.Recursive);
                }
            }
        }

        public static void Hide(GameObject gameObject,
            List<GraphicComponentStateSnapshot> componentSnapshots,
            IShowable.Settings settings = IShowable.Settings.Default)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            CC.Guard.IsNotNull(componentSnapshots, nameof(componentSnapshots));

            var graphics = gameObject.GetComponentsInChildren<Graphic>().Where(cmp => cmp is not IShowable);

            if (graphics.IsEmpty())
                return;

            componentSnapshots.Clear();

            foreach (var cmp in graphics)
            {
                componentSnapshots.Add(new GraphicComponentStateSnapshot(cmp));

                if (!settings.IsFlagSetted(IShowable.Settings.KeepRaycastTargetState))
                    cmp.raycastTarget = false;

                cmp.color = cmp.color.WithAlpha(0f);

                if (cmp.gameObject != gameObject)
                    cmp.enabled = false;
            }

            if (settings.IsFlagSetted(IShowable.Settings.Recursive))
            {
                foreach (var cmp in gameObject.FindFor()
                    .InChildren()
                    .Transforms()
                    .Select(child => child.AsOrDefault<IShowable>())
                    .Where(showable => showable.IsSome)
                    .Select(showable => showable.AccessUnsafe()))
                {
                    cmp.Hide(settings & ~IShowable.Settings.Recursive);
                }
            }
        }
    }
}
