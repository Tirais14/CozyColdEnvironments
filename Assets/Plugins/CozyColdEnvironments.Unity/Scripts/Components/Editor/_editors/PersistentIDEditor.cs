#if UNITY_EDITOR
using CCEnvs.UnityX.Editr;
using UnityEditor;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.Components.Editr
{
    [CustomEditor(typeof(PersistentID))]
    public class PersistentIDEditor : CCEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();

            var space = new VisualElement();
            space.style.minHeight = 20f;
            root.Add(space);

            var button = new Button(GenerateNewID)
            {
                text = "Generate ID"
            };

            button.style.minHeight = 15f;

            root.Add(button);
            return root;
        }

        private void GenerateNewID()
        {
            var id = PersistentID.GetNextID();
            target.CastTo<PersistentID>().Value = id;
        }
    }
}
#endif