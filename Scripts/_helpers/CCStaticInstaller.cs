#nullable enable
using CCEnvs.Attributes;
using CCEnvs.Common;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CCEnvs
{
    public static class CCStaticInstaller
    {
        public static void Install()
        {
            IEnumerable<MethodInfo> installers =
                from x in AppDomain.CurrentDomain.GetAssemblies()
                select x.GetModules() into modules
                from m in modules
                select m.FindTypes(
                    (type, attributeType) => type.IsDefined((Type)attributeType, inherit: true),
                    typeof(CCStaticInstallerAttribute)) into types
                where types.IsNotNullOrEmpty()
                from t in types
                select t.ForceGetMethods(BindingFlagsDefault.StaticAll) into methods
                from mt in methods
                where mt.IsDefined<StaticInstallerMethodAttribute>(inherit: true)
                ||
                mt.Name.Equals("Install", StringComparison.Ordinal)
                select mt;

            foreach (var installer in installers)
            {
                installer.Invoke(obj: null, Array.Empty<object>());
                CCDebug.PrintLog($"{installer.ReflectedType.Namespace}.{installer.ReflectedType.GetName()} executed.");
            }
        }
    }
}
