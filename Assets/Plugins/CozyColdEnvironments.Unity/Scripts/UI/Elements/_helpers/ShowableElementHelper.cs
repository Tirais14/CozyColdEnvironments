using CommunityToolkit.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public static class ShowableElementHelper
    {
        public static bool TryGetRootOfRenderer<TShowable>(
            PanelRenderer renderer,
            TShowable showable,
            [NotNullWhen(true)] out IShowableElement? result
            )
            where TShowable : Component, IShowableElement
        {
            CC.Guard.IsNotNull(renderer, nameof(renderer));
            CC.Guard.IsNotNull(showable, nameof(showable));

            Transform root = showable.transform.root;

            if (root == null || root == showable.transform)
            {
                result = null;
                return false;
            }

            foreach (var otherRenderer in root.Q().FromChildrens().Components<PanelRenderer>())
            {
                if (showable == otherRenderer)
                    break;

                if (renderer == otherRenderer)
                {
                    if (otherRenderer.Q()
                        .Component<IShowableElement>()
                        .Lax()
                        .TryGetValue(out var rootShowable))
                    {
                        result = rootShowable;
                        return true;
                    }

                    break;
                }
            }

            result = null;
            return false;
        }

        public static void InitShowable<TShowable>(
            PanelRenderer renderer,
            TShowable showable,
            PanelRenderer.UIReloadCallback uiReloadCallback
            )
            where TShowable : Component, IShowableElement
        {
            CC.Guard.IsNotNull(renderer, nameof(renderer));
            CC.Guard.IsNotNull(showable, nameof(showable));
            Guard.IsNotNull(uiReloadCallback);

            if (!TryGetRootOfRenderer(renderer, showable, out IShowableElement? root))
                renderer.RegisterUIReloadCallback(uiReloadCallback);
        }
    }
}
