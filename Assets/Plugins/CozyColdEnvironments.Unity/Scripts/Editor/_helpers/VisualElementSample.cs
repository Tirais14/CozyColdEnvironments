#if UNITY_EDITOR
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    public static class VisualElementSample
    {
        public static VisualElement Empty(float size = 15f)
        {
            var t = new VisualElement();

            t.style.height = size;

            return t;
        }
    }
}
#endif