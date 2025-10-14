using Humanizer;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    [Obsolete("Not finished", true)]
    public sealed class ImportAsTiles : EditorWindow
    {
        public const string DEFAULT_INPUT_DIRECTORY_PREF = nameof(ImportAsTiles) + "_DefaultInputDirectory";
        public const string DEFAULT_OUTPUT_DIRECTORY_PREF = nameof(ImportAsTiles) + "_DefaultOutputDirectory";

        private Toggle recursiveMode = null!;
        private Button selectDefaultInputDirectory = null!;
        private TextField selectedDefaultInputDirectory = null!;

        private Button selectDefaulOutputDirectory = null!;
        private TextField selectedDefaultOutputDirectory = null!;

        private Button selectImages = null!;

        private string selectedInputFolder = string.Empty;
        private string selectedOutputFolder = string.Empty;

        //[MenuItem("Import/As Tiles")]
        public static void ShowWindow()
        {
            GetWindow<ImportAsTiles>(
                nameof(ImportAsTiles).Underscore()
                                     .Humanize(LetterCasing.Sentence));
        }

        public void CreateGUI()
        {
            ConstructUIElements();
            AddElements();
        }

        private void SelectImages()
        {
            selectedInputFolder = EditorUtility.OpenFilePanel(
                "Select input folder",
                selectedDefaultInputDirectory.value,
                string.Empty);

            selectedOutputFolder = EditorUtility.OpenFilePanel(
                "Select input folder",
                selectedDefaultOutputDirectory.value,
                string.Empty);

            var inputDir = new DirectoryInfo(selectedInputFolder);

            FileInfo[] files = inputDir.GetFiles("*", SearchOption.AllDirectories);

            //files.Where(file => file.Extension.EqualsOrdinal(".png", ignoreCase: true)).Select(file => Texture2D.CreateExternalTexture())
        }

        private void SelectDefaulInputDirectory()
        {
            selectedDefaultInputDirectory.value = EditorUtility.OpenFolderPanel(
                nameof(selectDefaultInputDirectory).Underscore()
                                                  .Humanize(LetterCasing.Sentence),
                Application.dataPath,
                string.Empty);

            EditorPrefs.SetString(
                DEFAULT_INPUT_DIRECTORY_PREF,
                selectedDefaultInputDirectory.value);
        }

        private void SelectDefaulOutputDirectory()
        {
            selectedDefaultInputDirectory.value = EditorUtility.OpenFolderPanel(
                nameof(selectedDefaultOutputDirectory).Underscore()
                                                      .Humanize(LetterCasing.Sentence),
                Application.dataPath,
                string.Empty);

            EditorPrefs.SetString(
                DEFAULT_OUTPUT_DIRECTORY_PREF,
                selectedDefaultOutputDirectory.value);
        }

        private void ConstructUIElements()
        {
            recursiveMode = new Toggle()
            {
                label = nameof(recursiveMode).Underscore()
                                             .Humanize(LetterCasing.Sentence),
                value = true,
            };

            selectDefaultInputDirectory = new Button(SelectDefaulInputDirectory)
            {
                text = nameof(selectDefaultInputDirectory).Underscore()
                                                     .Humanize(LetterCasing.Sentence),
            };

            selectedDefaultInputDirectory = new TextField()
            {
                label = nameof(selectedDefaultInputDirectory).Underscore()
                                                        .Humanize(LetterCasing.Sentence),
                value = EditorPrefs.GetString(DEFAULT_INPUT_DIRECTORY_PREF) ?? string.Empty,
                isReadOnly = true
            };

            selectDefaulOutputDirectory = new Button(SelectDefaulOutputDirectory)
            {
                text = nameof(selectDefaulOutputDirectory).Underscore()
                                         .Humanize(LetterCasing.Sentence),
            };

            selectedDefaultOutputDirectory = new TextField()
            {
                label = nameof(selectedDefaultOutputDirectory).Underscore()
                                                        .Humanize(LetterCasing.Sentence),
                value = EditorPrefs.GetString(DEFAULT_OUTPUT_DIRECTORY_PREF) ?? string.Empty,
                isReadOnly = true
            };

            selectImages = new Button(SelectImages)
            {
                text = nameof(selectImages).Underscore()
                                           .Humanize(LetterCasing.Sentence),
            };
        }

        private void AddElements()
        {
            rootVisualElement.Add(selectedDefaultInputDirectory);
            rootVisualElement.Add(selectDefaultInputDirectory);
            rootVisualElement.Add(VisualElementSample.Empty());

            rootVisualElement.Add(selectedDefaultOutputDirectory);
            rootVisualElement.Add(selectDefaulOutputDirectory);
            rootVisualElement.Add(VisualElementSample.Empty());

            rootVisualElement.Add(selectImages);
        }
    }
}
