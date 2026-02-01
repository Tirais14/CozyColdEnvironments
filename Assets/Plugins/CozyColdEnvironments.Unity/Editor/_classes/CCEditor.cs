using CCEnvs.UnityEditor;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    public class CCEditor : global::UnityEditor.Editor
    {
        private VisualElement root = null!;

        protected static bool IsNotVisibleField(FieldInfo targetField)
        {
            return targetField.IsDefined(typeof(HideInInspector))
                   ||
                   !targetField.IsPublic
                   &&
                   !targetField.IsDefined(typeof(SerializeField));
        }

        private static bool IsScriptProperty(string propertyName)
        {
            return propertyName == "m_Script";
        }

        private void SetupScriptProperty(SerializedProperty prop)
        {
            PropertyField scriptField = new(prop);
            scriptField.SetEnabled(false);
            root.Add(scriptField);
        }

        public override VisualElement CreateInspectorGUI()
        {
            root ??= new VisualElement();

            if (target == null)
                return root;

            SerializedObject serializedObject = new(target);
            SerializedProperty prop = serializedObject.GetIterator();
            PropertyField propField;
            bool enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (IsScriptProperty(prop.name))
                    SetupScriptProperty(prop);

                if (GetTargetField(prop.name) is not FieldInfo propUnderlyingField)
                    continue;

                if (!IsSerializedProperty(prop, propUnderlyingField))
                    continue;

                propField = CreatePropertyField(target, prop, propUnderlyingField);

                root.Add(propField);
            }

            // Apply changes
            root.TrackSerializedObjectValue(serializedObject, _ => serializedObject.ApplyModifiedProperties());

            EditorHelper.AddUIElements(root, this);

            return root;
        }

        protected virtual PropertyField CreatePropertyField(
            Object target,
            SerializedProperty prop,
            FieldInfo propUnderlyingField
            )
        {
            return new PropertyField(prop.Copy())
            {
                label = prop.displayName
            };
        }

        protected virtual bool IsSerializedProperty(
            SerializedProperty prop,
            FieldInfo propUnderlyingField
            )
        {
            return !IsNotVisibleField(propUnderlyingField);
        }

        private FieldInfo? GetTargetField(string fieldName)
        {
            return target.GetType().
                GetField(fieldName, BindingFlagsDefault.InstanceAll);
        }
    }
}
