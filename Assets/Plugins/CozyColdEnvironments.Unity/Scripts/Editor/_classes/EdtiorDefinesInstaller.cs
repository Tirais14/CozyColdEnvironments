#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCEnvs.UnityEditor;
using SuperLinq;
using UnityEditor;
using UnityEditor.Build;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    public static class EdtiorDefinesInstaller
    {
        private const string UNITASK = "Cysharp.Threading";
        private const string ZLINQ = "ZLinq";
        private const string ADDRESSABLES = "AddressableAssets";
        private const string ZENJECT = "Zenject";

        private const string UNITASK_SYMBOL = "UNITASK_PLUGIN";
        private const string ZLINQ_SYMBOL = "ZLINQ_PLUGIN";
        private const string ADDRESSABLES_SYMBOL = "ADDRESSABLES_PLUGIN";
        private const string ZENJECT_SYMBOL = "ZENJECT_PLUGIN";

        private readonly static Dictionary<string, string[]> nspaceDefineSymbols = new()
        {
            { UNITASK, Range.From(UNITASK_SYMBOL) },
            { ZLINQ, Range.From(ZLINQ_SYMBOL) },
            { ADDRESSABLES, Range.From(ADDRESSABLES_SYMBOL) },
            { ZENJECT, Range.From(ZENJECT_SYMBOL) }
        };

        [MenuItem(EditorHelper.COMPILING_TAB_NAME + "/" + EditorHelper.CC_TAB + "/Add Define Symbols", priority = -1)]
        public static void AddDefines()
        {
            var nspaces = GetInstalledPluginNamespaces();

            var groupDefineSymbols = GetTargetGroupDefineSymbols();

            RemoveDefines();

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

        [MenuItem(EditorHelper.COMPILING_TAB_NAME + "/" + EditorHelper.CC_TAB + "/Remove Define Symbols", priority = -1)]
        public static void RemoveDefines()
        {
            foreach (var targetGroup in GetTargetGroupDefineSymbols())
            {
                foreach (var defineSymbol in nspaceDefineSymbols.Values.SelectMany(x => x))
                {
                    targetGroup.Value.Remove(defineSymbol);
                }
            }

            foreach (var (bTarget, symbol) in GetPlatformDefineSymbols())
                PlayerSettingsHelper.RemoveScriptingDefineSymbols(Range.From(bTarget), symbol);
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

        private static Type[] GetAssemblyTypes()
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies().AsParallel()
                    select assembly.GetTypes() into types
                    from type in types
                    where type.Namespace is not null
                    select type
                    )
                    .ToArray();
        }

        private static HashSet<string> GetInstalledPluginNamespaces()
        {
            var types = GetAssemblyTypes();

            var results = new HashSet<string>();

            Parallel.ForEach(types,
                (type, loopState) =>
                {
                    if (type.Namespace is null)
                        return;

                    if (type.Namespace.StartsWith(UNITASK))
                        results.Add(UNITASK);
                    else if (type.Namespace.StartsWith(ZLINQ))
                        results.Add(ZLINQ);
                    else if (type.Namespace.StartsWith(ADDRESSABLES))
                        results.Add(ADDRESSABLES);
                    else if (type.Namespace.StartsWith(ZENJECT))
                        results.Add(ZENJECT);

                    if (results.Count == 4)
                        loopState.Break();
                });

            return results;
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
