#if UNITY_EDITOR
using CCEnvs.UnityX.Components;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.Editr
{
    [CustomEditor(typeof(PersistentGuid))]
    public class PersistentGUIDEditor : CCEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();
            var separator = new VisualElement();
            separator.style.minHeight = 20f;
            root.Add(separator);

            var generateBtn = new Button(() => target.CastTo<PersistentGuid>().GenerateGuid())
            {
                text = "Generate ID",
                tooltip = $"To regenerate GUID button must be pressed {target.CastTo<PersistentGuid>().PressCountToRegenerate} times"
            };
            generateBtn.style.minHeight = 15f;
            root.Add(generateBtn);

            return root;
        }
    }
}
#endif