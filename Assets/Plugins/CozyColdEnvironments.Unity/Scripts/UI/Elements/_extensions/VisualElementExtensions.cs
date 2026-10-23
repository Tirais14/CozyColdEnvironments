using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public static class VisualElementExtensions
    {
        public static T QStrict<T>(this VisualElement element, string? name = null)
            where T : VisualElement
        {
            return element.Q<T>(name) ??
                throw new System.InvalidOperationException(DebugMessageBuilder.CreatePooled()
                .AddMessage("Not found element")
                .AddProperty("SearchRoot", element)
                .AddProperty("Name", name)
                .AddProperty("Type", typeof(T))
                .ToStringAndDispose()
                );
        }
        public static T QStrict<T>(this VisualElement element, string? name = null, string? className = null)
            where T : VisualElement
        {
            return element.Q<T>(name, className) ??
                throw new System.InvalidOperationException(DebugMessageBuilder.CreatePooled()
                .AddMessage("Not found element")
                .AddProperty("SearchRoot", element)
                .AddProperty("Name", name)
                .AddProperty("Type", typeof(T))
                .AddPredicatedProperty(className.IsNotNullOrWhiteSpace(), "Class", className)
                .ToStringAndDispose()
                );
        }
        public static T QStrict<T>(this VisualElement element, string? name = null, params string[]? classNames)
            where T : VisualElement
        {
            classNames ??= Array.Empty<string>();

            return element.Q<T>(name, classNames) ??
                throw new System.InvalidOperationException(DebugMessageBuilder.CreatePooled()
                .AddMessage("Not found element")
                .AddProperty("SearchRoot", element)
                .AddProperty("Name", name)
                .AddProperty("Type", typeof(T))
                .AddPredicatedProperty(classNames.Length >= 1, "Classes", classNames.SequenceToString())
                .ToStringAndDispose()
                );
        }
    }
}
