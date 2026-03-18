#if UNITY_EDITOR
using System.Reflection;
using CCEnvs.UnityEditor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.Unity.UnityEditor
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

        public override VisualElement CreateInspectorGUI()
        {
            root ??= new VisualElement();

            if (target == null)
                return root;

            SerializedObject serializedObject = new(target);

            SerializedProperty prop = serializedObject.GetIterator();

            VisualElement propField;

            bool enterChildren = true;

            int i = 0;

            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (IsScriptProperty(prop.name))
                    SetupScriptProperty(prop);

                if (GetTargetField(prop.name) is not FieldInfo propUnderlyingField)
                    continue;

                if (!IsSerializedProperty(prop, propUnderlyingField))
                    continue;

                propField = CreateElement(
                    target,
                    prop,
                    propUnderlyingField,
                    i++
                    );

                root.Add(propField);
            }

            // Apply changes
            root.TrackSerializedObjectValue(serializedObject, _ => serializedObject.ApplyModifiedProperties());

            EditorHelper.AddUIElements(root, this);

            return root;
        }

        protected virtual VisualElement CreateElement(
            Object target,
            SerializedProperty prop,
            FieldInfo propUnderlyingField,
            int pointer
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
    }
}
#endif