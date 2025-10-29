using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public static class Showable
    {
        //TODO: Unbind logic from game object activation
        public static void Show<T>(T target)
            where T : Component, IShowable
        {
            CC.Guard.IsNotNull(target, nameof(target));

            if (target.IsVisible)
                return;

            UIHelper.EnableGraphics(target);
        }

        //TODO: Unbind logic from game object activation
        public static void Hide<T>(T target)
            where T : Component, IShowable
        {
            CC.Guard.IsNotNull(target, nameof(target));

            if (!target.IsVisible)
                return;

            UIHelper.DisableGraphics(target);
        }
    }
}
