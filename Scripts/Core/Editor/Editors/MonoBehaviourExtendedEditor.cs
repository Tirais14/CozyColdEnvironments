using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UTIRLib.Attributes;
using UTIRLib.ComponentSetter;

#nullable enable

namespace UTIRLib.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoX), editorForChildClasses: true)]
    public class MonoBehaviourExtendedEditor : UnityEditor.Editor
    {
        private VisualElement root = null!;

        public override VisualElement CreateInspectorGUI()
        {
            root ??= new VisualElement();

            SerializedObject serializedObject = new(target);

            SerializedProperty prop = serializedObject.GetIterator();
            bool enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (IsCoreComponentSerializedProperty(prop.name))
                {
                    SetupCoreComponentSerializedProperty(prop);
                }

                if (GetTargetField(prop.name) is not FieldInfo targetField) continue;

                if (IsNotVisibleField(targetField)) continue;

                PropertyField propertyField = new(prop){
                    label = prop.displayName
                };

                ProccessOptionalAttribute(targetField, propertyField);
                ProccessGetComponentAttribute(targetField, propertyField);

                root.Add(propertyField);
            }

            // Apply changes
            root.TrackSerializedObjectValue(serializedObject, _ => serializedObject.ApplyModifiedProperties());

            return root;
        }

        private FieldInfo? GetTargetField(string fieldName) =>
            target.
            GetType().
            GetField(fieldName, BindingFlagsDefault.InstanceAll.ToBindingFlags());

        private static bool IsCoreComponentSerializedProperty(string propertyName) => propertyName == "m_Script";

        private void SetupCoreComponentSerializedProperty(SerializedProperty prop)
        {
            PropertyField scriptField = new(prop);
            scriptField.SetEnabled(false);
            root.Add(scriptField);
        }

        private static bool IsNotVisibleField(FieldInfo targetField) => targetField.IsDefined(typeof(HideInInspector))
                || !targetField.IsDefined(typeof(SerializeField));

        private static void ProccessOptionalAttribute(FieldInfo targetField, PropertyField propertyField)
        {
            if (targetField.GetCustomAttribute(typeof(OptionalAttribute)) is OptionalAttribute)
            {
                propertyField.label = "⭕" + propertyField.label;
                propertyField.style.backgroundColor = new Color(0f, 0f, 1f, 0.05f);
            }
        }

        private static void ProccessGetComponentAttribute(FieldInfo targetField, PropertyField propertyField)
        {
            if (targetField.GetCustomAttribute(typeof(GetSelfAttribute))
                is GetSelfAttribute getComponentAttribute)
            {
                propertyField.label = getComponentAttribute switch {
                    GetComponentInChildrenAttribute => $"[🧩▼]" + propertyField.label,
                    GetByParentAttribute => $"[🧩▲]" + propertyField.label,
                    _ => $"[🧩]" + propertyField.label,
                };
            }
        }
    }
}