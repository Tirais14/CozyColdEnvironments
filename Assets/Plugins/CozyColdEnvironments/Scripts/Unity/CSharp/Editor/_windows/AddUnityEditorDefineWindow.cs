using CCEnvs.Attributes;
using CCEnvs.Unity.EditorC;
using System;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.Unity.CSharp.Editor
{
    public sealed class AddUnityEditorDefineWindow : CCEditorWindow
    {
        private const string NSPACE_KEYS_DEFAULT_TEXT = "first,seconds...";

        [OnInstallResetable]
        private static bool inProcess;

        private TextField folderPath = null!;

        private TextField byFolders = null!;

        private Button processBtn = null!;

        private void OnDestroy()
        {
            processBtn.clicked -= AddDefines;
        }

        [MenuItem("Window/CCExtensions/Add Unity Editor Defines")]
        public static void GetWindow()
        {
            GetWindow<AddUnityEditorDefineWindow>(); 
        }

        protected override void CreateElements()
        {
            folderPath = new TextField("Folder Path");

            processBtn = new Button()
            {
                text = "Add Defines"
            };

            processBtn.clicked += AddDefines;

            byFolders = new TextField("By Folders")
            {
                value = NSPACE_KEYS_DEFAULT_TEXT
            };
        }

        private async void AddDefines()
        {
            if (inProcess)
            {
                this.PrintError("Already in process");
                return;
            }
            if (folderPath.text.IsNullOrWhiteSpace())
            {
                this.PrintError("Invalid Folder Path");
                return;
            }

            if (!Directory.Exists(folderPath.text))
            {
                this.PrintError($"Directory doesn't exists. Path: {folderPath.text}");
                return;
            }

            inProcess = true;

            try
            {
                await EditorScriptHelper.AddUnityEditorDefineSymbol(
                    folderPath.text,
                    byFolders.value.Split(',')
                    );
            }
            finally
            {
                inProcess = false;
            }
        }
    }
}
