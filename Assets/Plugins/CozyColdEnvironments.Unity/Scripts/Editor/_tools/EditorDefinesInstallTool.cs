#if UNITY_EDITOR
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;

#nullable enable
namespace CCEnvs.Unity.Editr
{
    [InitializeOnLoad]
    public static class EditorDefinesInstallTool
    {
        private const string UNITASK = "Cysharp.Threading";
        private const string ZLINQ = "ZLinq";
        private const string ADDRESSABLES = "AddressableAssets";
        private const string ZENJECT = "Zenject";
        private const string VCONTAINER = "VContainer";
        private const string BIN_PACKER_EB_AFIT = "CromulentBisgetti";
        private const string SPLINES = "UnityEngine.Splines";

        private const string UNITASK_SYMBOL = "UNITASK_PLUGIN";
        private const string ZLINQ_SYMBOL = "ZLINQ_PLUGIN";
        private const string ADDRESSABLES_SYMBOL = "ADDRESSABLES_PLUGIN";
        private const string ZENJECT_SYMBOL = "ZENJECT_PLUGIN";
        private const string VCONTAINER_SYMBOL = "VCONTAINER_PLUGIN";
        private const string BIN_PACKER_EB_AFIT_SYMBOL = "BIN_PACKER_EB_PLUGIN";
        private const string SPLINES_SYMBOL = "SPLINES_PLUGIN";

        private readonly static Dictionary<string, string[]> nspaceDefineSymbols = new()
        {
            { UNITASK, Range.From(UNITASK_SYMBOL) },
            { ZLINQ, Range.From(ZLINQ_SYMBOL) },
            { ADDRESSABLES, Range.From(ADDRESSABLES_SYMBOL) },
            { ZENJECT, Range.From(ZENJECT_SYMBOL) },
            { VCONTAINER, Range.From(VCONTAINER_SYMBOL) },
            { BIN_PACKER_EB_AFIT, Range.From(BIN_PACKER_EB_AFIT_SYMBOL) },
            { SPLINES, Range.From(SPLINES_SYMBOL) }
        };

        static EditorDefinesInstallTool()
        {

        }

        [MenuItem(EditorHelper.TOOLS_TAB_NAME + "/" + EditorHelper.CCENVS_TAB + "/Install", priority = -1)]
        public static void AddDefines()
        {
            var nspaces = GetInstalledPluginNamespaces();

            var groupDefineSymbols = GetTargetGroupDefineSymbols();

            RemoveDefines();

            try
            {
                foreach (var nspace in nspaces)
                {
                    foreach (var targetGroup in groupDefineSymbols.Keys)
                    {
                        var symbols = groupDefineSymbols[targetGroup];

                        foreach (var defineSymbol in nspaceDefineSymbols[nspace])
                        {
                            symbols.Add(defineSymbol);
                        }

                        PlayerSettings.SetScriptingDefineSymbols(targetGroup, symbols.JoinStrings(';'));
                    }
                }

                foreach (var (bTarget, symbol) in GetPlatformDefineSymbols())
                    PlayerSettingsHelper.AddScriptingDefineSymbols(Range.From(bTarget), symbol);
            }
            catch (Exception ex)
            {
                typeof(EditorDefinesInstallTool).PrintException(ex);
            }
        }

        [MenuItem(EditorHelper.TOOLS_TAB_NAME + "/" + EditorHelper.CCENVS_TAB + "/Uninstall", priority = -1)]
        public static void RemoveDefines()
        {
            foreach (var targetGroup in GetTargetGroupDefineSymbols())
                foreach (var defineSymbol in nspaceDefineSymbols.Values.SelectMany(x => x))
                {
                    targetGroup.Value.Remove(defineSymbol);
                    PlayerSettingsHelper.RemoveScriptingDefineSymbols(Range.From(targetGroup.Key), defineSymbol);
                }
        }

        private static (NamedBuildTarget bTarget, string symbol)[] GetPlatformDefineSymbols()
        {
            return new (NamedBuildTarget bTarget, string symbol)[]
            {
                (NamedBuildTarget.Android, "ANDROID_PLATFORM"),
                (NamedBuildTarget.iOS, "IOS_PLATFORM"),
                (NamedBuildTarget.Standalone, "STANDALONE_PLATFORM"),
            };
        }

        private static string[] GetInstalledPluginNamespaces()
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies().AsParallel()
                    select assembly.GetTypes() into types
                    from type in types
                    where type.Namespace is not null
                    select nspaceDefineSymbols.Select(nspace => type.Namespace.StartsWith(nspace.Key) ? nspace.Key : null).FirstOrDefault(nspace => nspace is not null) into nspace
                    where nspace is not null
                    select nspace
                    )
                    .DistinctBy(nspace => nspace.Split('.')[0])
                    .ToArray();
        }

        private static Dictionary<NamedBuildTarget, HashSet<string>> GetTargetGroupDefineSymbols()
        {
            return PlayerSettingsHelper.GetNamedBuildTargets()
                .Select(bTarget => (bTarget, PlayerSettingsHelper.GetScriptingDefineSymbols(bTarget)))
                .ToDictionary();
        }
    }
}
#endif
