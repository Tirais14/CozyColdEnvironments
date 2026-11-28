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

            var generateButton = new Button(GenerateNewID)
            {
                text = "Generate ID"
            };

            generateButton.style.minHeight = 15f;
            root.Add(generateButton);

            var eraseButton = new Button(EraseID)
            {
                text = "Erase ID"
            };
            eraseButton.style.minHeight = 15f;
            root.Add(eraseButton);

            return root;
        }

        private void GenerateNewID()
        {
            var id = PersistentID.GetNextID();
            target.CastTo<PersistentID>().Value = id;
        }

        private void EraseID()
        {
            var id = target.CastTo<PersistentID>();
            PersistentID.Unregister(id.Value);
            id.value = null;
        }
    }
}
#endif