using CCEnvs.Diagnostics;
using UnityEditor;
using System.Linq;
using System;
using CCEnvs.Reflection;
using CCEnvs.Unity.Attributes;
using SuperLinq;

#nullable enable
namespace CCEnvs.Unity
{
    public static class EditorDomain
    {
        public static bool IsReloaded { get; private set; }

        [InitializeOnLoadMethod]
        public static void OnDomainReload()
        {
            IsReloaded = true;

            CCDebug.PrintLog($"{nameof(IsReloaded)} = true",
                             new DebugContext(typeof(EditorDomain)).Additive().Editor());
        }

        [InitializeOnEnterPlayMode]
        public static void OnPlayModeEnter()
        {
            IsReloaded = false;

            InvokeOnEnterPlayModeResetMethods();

            CCDebug.PrintLog($"{nameof(IsReloaded)} = false",
                             new DebugContext(typeof(EditorDomain)).Additive().Editor());
        }

        private static void InvokeOnEnterPlayModeResetMethods()
        {
            //(from assembly in AppDomain.CurrentDomain.GetAssemblies()
            // select assembly.GetTypes() into types
            // from type in types
            // select type.ForceGetMethods(BindingFlagsDefault.StaticAll) into methods
            // from method in methods
            // where method.IsDefined<ResetOnEnterPlayModeAttribute>(inherit: true)
            // where method.GetParameters().IsEmpty()
            // select method)
            // .ForEach(method => method.Invoke(obj: null, CC.EmptyArguments));
        }
    }
}
