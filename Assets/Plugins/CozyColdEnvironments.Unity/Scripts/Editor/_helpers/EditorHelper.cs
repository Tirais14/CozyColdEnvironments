using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

#nullable enable

namespace CCEnvs.UnityEditor
{
    public static class EditorHelper
    {
        public const string MAIN_TAB_NAME = "CC";
        public const string EDITOR_TAB_NAME = "Editor";
        public const string TOOLS_TAB_NAME = "Tools";
        public const string COMPILING_TAB_NAME = "Compiling";
        public const string IDE_TAB_NAME = "IDE";
        public const string WINDOWS_TAB_NAME = "Window";
        public const string BUILD_TAB_NAME = "Build";
        public const string CONTEXT_MENU_TAB = "Assets";
        public const string CONTEXT_MENU_CREATE_TAB = "Create";
        public const string CONTEXT_MENU_CREATE_PATH = CONTEXT_MENU_TAB + "/" + CONTEXT_MENU_CREATE_TAB;

        [Obsolete]
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddUIElementsByReflection(
            Type type, 
            object editorInstance,
            VisualElement root
            )
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

        public static void AddUIElements(VisualElement root, object instance)
        {
            foreach (var item in from field in instance.Reflect().IncludeNonPublic().Fields()
                                 where field.FieldType.IsType<VisualElement>()
                                 select field.GetValue(instance).To<VisualElement>())
            {
                if (root.Contains(item))
                    continue;

                root.Add(item);
            }
        }
    }
}