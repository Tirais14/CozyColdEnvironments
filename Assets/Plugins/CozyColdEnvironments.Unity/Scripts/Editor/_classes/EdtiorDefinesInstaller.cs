#if UNITY_EDITOR
using CCEnvs.UnityEditor;
using CCEnvs.Utils;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    public static class EdtiorDefinesInstaller
    {
        private const string UNITASK = "UniTask";
        private const string ZLINQ = "ZLinq";
        private const string ADDRESSABLES = "AddressableAssets";

        private const string UNITASK_SYMBOL = "UNITASK_PLUGIN";
        private const string UNITASK_SYMBOL_ALT = "UNITASK_PLUGIN";
        private const string ZLINQ_SYMBOL = "ZLINQ_PLUGIN";
        private const string ZLINQ_SYMBOL_ALT = "ZLINQ";
        private const string ADDRESSABLES_SYMBOL = "ADDRESSABLES_PLUGIN";
        private const string ADDRESSABLES_SYMBOL_ALT = "ADDRESSABLES_PLUGIN";

        private readonly static Dictionary<string, string[]> nspaceDefineSymbols = new()
        {
            { UNITASK, Range.From(UNITASK_SYMBOL) },
            { ZLINQ, Range.From(ZLINQ_SYMBOL) },
            { ADDRESSABLES, Range.From(ADDRESSABLES_SYMBOL) }
        };

        [MenuItem(EditorHelper.BUILD_TAB_NAME + "/" + EditorHelper.MAIN_TAB_NAME + "/Add Defines")]
        public static void AddDefines()
        {
            var nspaces = GetInstalledPluginNamespaces();

            var targetGroups = EnumCache<BuildTargetGroup>.Values.Select(
                static targetGroup =>
                {
                    try
                    {
                        return NamedBuildTarget.FromBuildTargetGroup(targetGroup);
                    }
                    catch (System.Exception)
                    {
                        return NamedBuildTarget.Unknown;
                    }
                })
                .Where(targetGroup =>
                {
                    return targetGroup != NamedBuildTarget.Unknown;
                });

            var groupDefineSymbols = GetTargetGroupDefineSymbols();

            RemoveDefines();

            foreach (var nspace in nspaces)
            {
                foreach (var targetGroup in targetGroups)
                {
                    var symbols = groupDefineSymbols[targetGroup];

                    foreach (var defineSymbol in nspaceDefineSymbols[nspace])
                    {
                        symbols.Add(defineSymbol);
                    }

                    PlayerSettings.SetScriptingDefineSymbols(targetGroup, symbols.JoinStrings(';'));
                }
            }
        }

        [MenuItem(EditorHelper.BUILD_TAB_NAME + "/" + EditorHelper.MAIN_TAB_NAME + "/Remove Defines")]
        public static void RemoveDefines()
        {
            foreach (var targetGroup in GetTargetGroupDefineSymbols())
            {
                foreach (var defineSymbol in nspaceDefineSymbols.Values.SelectMany(x => x))
                {
                    targetGroup.Value.Remove(defineSymbol);
                }
            }
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

                    if (type.Namespace.Contains(UNITASK))
                        results.Add(UNITASK);
                    else if (type.Namespace.Contains(ZLINQ))
                        results.Add(ZLINQ);
                    else if (type.Namespace.Contains(ADDRESSABLES))
                        results.Add(ADDRESSABLES);

                    if (results.Count == 3)
                        loopState.Break();
                });

            return results;
        }

        private static Dictionary<NamedBuildTarget, HashSet<string>> GetTargetGroupDefineSymbols()
        {
            var targetGroups = EnumCache<BuildTargetGroup>.Values.Select(
                static targetGroup =>
                {
                    try
                    {
                        return NamedBuildTarget.FromBuildTargetGroup(targetGroup);
                    }
                    catch (System.Exception)
                    {
                        return NamedBuildTarget.Unknown;
                    }
                })
                .Where(targetGroup =>
                {
                    return targetGroup != NamedBuildTarget.Unknown;
                })
                .Distinct();

            return (from targetGroup in targetGroups
                    select (targetGroup, symbolsString: PlayerSettings.GetScriptingDefineSymbols(targetGroup)) into defineSymbolsInfo
                    select (defineSymbolsInfo.targetGroup, symbols: defineSymbolsInfo.symbolsString.Split(';').ToHashSet()) into defineSymbolsInfo
                    select KeyValuePair.Create(defineSymbolsInfo.targetGroup, defineSymbolsInfo.symbols)
                    )
                    .ToDictionary();
        }
    }
}
#endif
