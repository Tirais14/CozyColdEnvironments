#if UNITY_EDITOR
using CCEnvs.Unity.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.Unity.UnityEditor
{
    [CustomEditor(typeof(PersistentGuid))]
    public class GameObjectPersistentGuidEditor : global::UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var targetTyped = (PersistentGuid)target;
            var serailizedObj = new SerializedObject(target);
            var prop = serailizedObj.GetIterator();

            bool enterChildren = true;
            PropertyField propField;
            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (prop.name == "m_Script")
                {
                    propField = new PropertyField(prop)
                    {
                        enabledSelf = false
                    };
                }
                else
                    propField = new PropertyField(prop);

                root.Add(propField);
            }

            var separator = new VisualElement();
            separator.style.minHeight = 20f;
            root.Add(separator);

            var tooltip = new HelpBox($"To regenerate GUID button must be pressed {targetTyped.PressCountToRegenerate} times", HelpBoxMessageType.Info);
            root.Add(tooltip);

            var generateBtn = new Button(() => target.CastTo<PersistentGuid>().GenerateGuid())
            {
                text = "Generate GUID"
            };
            generateBtn.style.minHeight = 15f;
            root.Add(generateBtn);

            return root;
        }
    }
}
#endif