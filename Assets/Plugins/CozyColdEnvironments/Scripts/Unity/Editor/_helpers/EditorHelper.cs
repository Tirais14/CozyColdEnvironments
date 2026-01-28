using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using CCEnvs.Reflection;

#nullable enable

namespace CCEnvs.UnityEditor
{
    public static class EditorHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddUIElementsByReflection(Type type, object editorInstance,
            VisualElement root)
        {
            Guard.IsNotNull(type);
            Guard.IsNotNull(editorInstance);
            CC.Guard.IsNotNull(root, nameof(root));

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

        public static Maybe<string> GetProjectActiveFolderPath()
        {
            var tryGetActiveFolderPathMethod = typeof(ProjectWindowUtil).GetMethod(
                "TryGetActiveFolderPath",
                BindingFlagsDefault.StaticAll,
                binder: null,
                types: Range.From(typeof(string).MakeByRefType()),
                Range.From(new ParameterModifier(1))
                );

            var prms = new object?[1] { null };
            tryGetActiveFolderPathMethod.Invoke(null, prms);

            return (string?)prms[0];
        }
    }
}