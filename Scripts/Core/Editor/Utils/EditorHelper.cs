using System;
using System.Reflection;
using UnityEngine.UIElements;

#nullable enable

namespace UTIRLib.Editor
{
    public static class EditorHelper
    {
        public static void AddUIElementsByReflection<T>(T editorInstance, VisualElement root) =>
            AddUIElementsByReflection(typeof(T), editorInstance, root);

        /// <exception cref="ArgumentNullException"></exception>
        public static void AddUIElementsByReflection(Type type, object editorInstance,
            VisualElement root)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (editorInstance == null)
            {
                throw new ArgumentNullException(nameof(editorInstance));
            }
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public |
                BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.GetValue(editorInstance) is VisualElement visualElement &&
                    visualElement != root)
                {
                    root.Add(visualElement);
                }
            }
        }
    }
}