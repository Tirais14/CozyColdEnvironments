using CCEnvs.UnityEditor;
using UnityEditor;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    public abstract class CCPropertyDrawer : PropertyDrawer
    {
        protected VisualElement root = null!;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            root = base.CreatePropertyGUI(property);

            root ??= new VisualElement();

            //EditorHelper.AddUIElements(root, this);

            return root;
        }
    }
}
