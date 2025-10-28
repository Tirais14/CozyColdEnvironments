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
            CC.Guard.NullArgument(target, nameof(target));

            if (target.IsVisible)
                return;

            target.gameObject.SetActive(true);
        }

        //TODO: Unbind logic from game object activation
        public static void Hide<T>(T target)
            where T : Component, IShowable
        {
            CC.Guard.NullArgument(target, nameof(target));

            if (!target.IsVisible)
                return;

            target.gameObject.SetActive(false);
        }
    }
}
