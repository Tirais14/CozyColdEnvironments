using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    public static class ViusalElementSample
    {
        public static VisualElement Separator(float size = 10f)
        {
            var t = new VisualElement();

            t.style.height = size;

            return t;
        }
    }
}
