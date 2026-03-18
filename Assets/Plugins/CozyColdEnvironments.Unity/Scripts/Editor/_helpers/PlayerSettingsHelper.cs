#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using CCEnvs.Collections;
using CCEnvs.Utils;
using CommunityToolkit.Diagnostics;
using UnityEditor;
using UnityEditor.Build;

#nullable enable
namespace CCEnvs.Unity.UnityEditor
{
    public static class PlayerSettingsHelper
    {
        public static NamedBuildTarget[] GetNamedBuildTargets()
        {
            return EnumCache<BuildTargetGroup>.Values.Select(
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
                .Distinct()
                .ToArray();
        }

        public static HashSet<string> GetScriptingDefineSymbols(NamedBuildTarget buildTarget)
        {
            Guard.IsFalse(
                buildTarget == NamedBuildTarget.Unknown,
                nameof(buildTarget),
                "Invalid build target"
                );

            return PlayerSettings.GetScriptingDefineSymbols(buildTarget)
                .Split(';')
                .ToHashSet();
        }

        public static void SetScriptingDefineSymbols(
            NamedBuildTarget buildTarget,
            IEnumerable<string> symbols
            )
        {
            Guard.IsFalse(
                buildTarget == NamedBuildTarget.Unknown,
                nameof(buildTarget),
                "Invalid build target"
                );

            CC.Guard.IsNotNull(symbols, nameof(symbols));

            var symbolsStr = string.Join(';', symbols);

            PlayerSettings.SetScriptingDefineSymbols(buildTarget, symbolsStr);
        }

        public static void AddScriptingDefineSymbols(
            NamedBuildTarget[]? buildTargets,
            params string[] symbols
            )
        {
            buildTargets ??= GetNamedBuildTargets();

            Guard.IsNotNull(symbols, nameof(symbols));

            if (buildTargets.IsEmpty())
                return;

            if (symbols.IsEmpty())
                return;

            foreach (var buildTarget in buildTargets)
            {
                var symbolsSet = GetScriptingDefineSymbols(buildTarget);

                foreach (var symbol in symbols)
                    symbolsSet.Add(symbol);

                SetScriptingDefineSymbols(buildTarget, symbolsSet);
            }
        }

        public static void RemoveScriptingDefineSymbols(
            NamedBuildTarget[]? buildTargets,
            params string[] symbols
            )
        {
            buildTargets ??= GetNamedBuildTargets();

            Guard.IsNotNull(symbols, nameof(symbols));

            if (buildTargets.IsEmpty())
                return;

            if (symbols.IsEmpty())
                return;

            foreach (var buildTarget in buildTargets)
            {
                var symbolsSet = GetScriptingDefineSymbols(buildTarget);

                foreach (var symbol in symbols)
                    symbolsSet.Remove(symbol);

                SetScriptingDefineSymbols(buildTarget, symbolsSet);
            }
        }
    }
}
#endif