using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;
using UTIRLib.FileSystem;

#nullable enable

namespace UTIRLib.Editor
{
    public abstract class TirLibEditorWindow : EditorWindow
    {
        protected Type windowType = null!;
        protected VisualElement root = null!;

        protected virtual void ConstructUIElements() => SetRoot();

        protected virtual void AddUIElements() => EditorHelper.AddUIElementsByReflection(GetType(), this, root);

        protected void SetRoot() => root ??= rootVisualElement;

        protected VisualElement GetEmptyElement(float height) => new() {
            style = { height = height }
        };

        protected VisualElement GetSeparatorElement(
            float height = 2,
            Color? backgroundColor = null,
            float marginTop = 5,
            float marginBottom = 5) => new() {
                style = {
                    height = height,
                    backgroundColor = backgroundColor ?? Color.gray,
                    marginTop = marginTop,
                    marginBottom = marginBottom
                }
            };

        protected static FSPath SelectDirectory(string? defaultPath = null,
                                                string title = "Select Directory",
                                                string? defaultName = null)
        {
            string selectedDirectory = EditorUtility.OpenFolderPanel(
                title,
                defaultPath ?? Application.dataPath,
                defaultName ?? string.Empty);

            return new FSPath(selectedDirectory);
        }

        protected static bool TrySelectDirectory(out FSPath selectedDirectory,
                                                 string? defaultPath = null,
                                                 string title = "Select Directory",
                                                 string? defaultName = null)
        {
            selectedDirectory = SelectDirectory(defaultPath, title, defaultName);

            return string.IsNullOrWhiteSpace(selectedDirectory);
        }

        protected void Abort(string? message = null)
        {
            TirLibDebug.Log($"{windowType.GetProccessedName()}: Proccess aborted. {message}", this);
        }

        protected bool TryAbortByDirectoryPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Abort($"Path: \"{path}\" is null or empty.");
                return true;
            }
            if (Directory.Exists(path))
            {
                Abort($"Selected file path: \"{path}\" doesn't exist.");
                return true;
            }

            return false;
        }
        protected bool TryAbortByFilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Abort($"Path: \"{path}\" is null or empty.");
                return true;
            }
            if (File.Exists(path))
            {
                Abort($"Selected file path: \"{path}\" doesn't exist.");
                return true;
            }

            return false;
        }

        protected virtual void CreateGUI()
        {
            try
            {
                windowType = GetType();
                ConstructUIElements();
                AddUIElements();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}