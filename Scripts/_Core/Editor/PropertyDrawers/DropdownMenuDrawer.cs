using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.Unity.Editor
{
    [CustomPropertyDrawer(typeof(DropdownMenuAttribute))]
    public class DropdownMenuDrawer : PropertyDrawer
    {
        private string[] _options = Array.Empty<string>();
        private bool _initialized;

        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, $"{label.text} is not a string field", MessageType.Error);
                return;
            }

            Initialize(property);

            EditorGUI.BeginChangeCheck();

            int selectedIndex = Array.IndexOf(_options, property.stringValue);
            if (selectedIndex < 0) selectedIndex = 0;

            Rect popupPosition = position;
            popupPosition = EditorGUI.PrefixLabel(popupPosition, label);

            selectedIndex = EditorGUI.Popup(
                popupPosition,
                selectedIndex,
                _options
            );

            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = _options[selectedIndex];
            }
        }

        private void Initialize(SerializedProperty prop)
        {
            if (_initialized) return;

            _initialized = true;
            DropdownMenuAttribute attr = (DropdownMenuAttribute)attribute;

            try
            {
                object targetObject = prop.serializedObject.targetObject;

                Debug.LogWarning(targetObject.GetType().GetName());

                MethodInfo menuItemsGetter = attr.MenuItemsGetter;
                object[]? args = null;
                if (attr.HasArgumentsGetter)
                {
                    args = (object[])attr.ArgumentsGetter.Invoke(targetObject, null);
                }

                //Вызываем метод
                object result = menuItemsGetter.Invoke(targetObject, null);

                //Конвертируем результат в массив строк
               _options = result switch
               {
                   string[] arr => arr,
                   List<string> list => list.ToArray(),
                   _ => Array.Empty<string>()
               };
            }
            catch (Exception e)
            {
                Debug.LogError($"Error getting options: {e.Message}");
                _options = Array.Empty<string>();
            }
        }
    }
}
