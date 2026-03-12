#if UNITY_EDITOR
using System.IO;
using CCEnvs.Attributes;
using CCEnvs.Unity.EditorC;
using CCEnvs.UnityEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.Unity.CSharp.Editor
{
    public sealed class AddUnityEditorDefineWindow : CCEditorWindow
    {
        private const string NSPACE_KEYS_DEFAULT_TEXT = "first,second...";

        [OnInstallResetable]
        private static bool inProcess;

        private TextField folderPath = null!;

        private TextField byFolders = null!;

        private Button processBtn = null!;

        private void OnDestroy()
        {
            processBtn.clicked -= AddDefines;
        }

        [MenuItem(EditorHelper.WINDOWS_TAB_NAME + "/" + EditorHelper.CCEnvs + "/Editor Defines Marker")]
        public static void GetWindow()
        {
            GetWindow<AddUnityEditorDefineWindow>("Editor Defines Marker");
        }

        //[MenuItem("Assets/Add Unity Editor Defines")]
        //public static void GetWindowByProjectBrowser()
        //{
        //    var instance = GetWindow<AddUnityEditorDefineWindow>();

        //    if (Selection.activeObject == null)
        //        return;

        //    var folderPath = EditorHelper.GetProjectActiveFolderPath().Raw;
        //    var objectPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        //    instance.folderPath.value = Path.Combine(folderPath, objectPath);
        //}

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
            processBtn.enabledSelf = false;
            this.PrintLog($"Adding editor define by path: {byFolders.value} in folders: {byFolders.value}");

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
                processBtn.enabledSelf = true;
                this.PrintLog("Editor defines added");
            }
        }
    }
}
#endif