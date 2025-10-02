#nullable enable
using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System;
using System.Linq;

namespace CCEnvs
{
    public static class Install
    {
        public static void Static()
        {
            if (!UnityEditorDomain.IsFirstPlayModeEntrance)
                return;

            foreach (var installer in from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                      select assembly.GetTypes() into types
                                      from type in types
                                      where type.IsDefined<StaticInstallerAttribute>()
                                      select type.ForceGetMethods(BindingFlagsDefault.StaticAll) into methods
                                      from method in methods
                                      where method.IsDefined<StaticInstallerMethodAttribute>(inherit: true)
                                      select method)
            {
                installer.Invoke(obj: null, Array.Empty<object>());
                CCDebug.PrintLog($"{installer.ReflectedType.GetFullName()} executed.", typeof(Install));
            }
        }
    }
}
