#nullable enable
using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CCEnvs
{
    public static class CCProjectHelper
    {
        public static bool IsInstalling { get; private set; }

        public static void StaticObsolete()
        {
            if (!UnityEditorDomain.IsFirstPlayModeEntrance)
                return;

            foreach (var installer in from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                      select assembly.GetTypes() into types
                                      from type in types
                                      where type.IsDefined<StaticInstallerAttribute>()
                                      select type.Reflect().IncludeNonPublic().IncludeStatic().Methods() into methods
                                      from method in methods
                                      where method.IsDefined<StaticInstallerMethodAttribute>(inherit: true)
                                      select method)
            {
                installer.Invoke(obj: null, Array.Empty<object>());
                CCDebug.Instance.PrintLog($"{installer.ReflectedType.GetFullName()} executed.", typeof(CCProjectHelper));
            }
        }

        public static async Task InstallAsync()
        {
            if (IsInstalling)
                throw new InvalidOperationException("Installing process already started");

            if (!UnityEditorDomain.IsFirstPlayModeEntrance)
                return;

            IsInstalling = true;

            try
            {
                var members = await Task.Run(GetMembers);

                await Task.Run(() => OnInstallExecuteMethods(members));
                await Task.Run(() => OnInstallResetFields(members));
            }
            finally
            {
                IsInstalling = false;
            }
        }

        public static void Install()
        {
            if (IsInstalling)
                throw new InvalidOperationException("Installing process already started");

            if (!UnityEditorDomain.IsFirstPlayModeEntrance)
                return;

            IsInstalling = true;

            try
            {
                var members = GetMembers();

                OnInstallExecuteMethods(members);
                OnInstallResetFields(members);
            }
            finally
            {
                IsInstalling = false;
            }
        }

        private static MemberInfo[] GetMembers()
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    where OnInstallAssemblyFilter(assembly)
                    select assembly.GetTypes() into types
                    from type in types
                    select type.GetMembers(BindingFlagsDefault.StaticAll) into mbms
                    from member in mbms
                    select member)
                    .ToArray();
        }

        private static void OnInstallExecuteMethods(MemberInfo[] members)
        {
            var methods = from member in members
                          where member.MemberType == MemberTypes.Method
                          select (MethodInfo)member into method
                          where method.IsDefined<OnInstallMethodAttribute>(inherit: false)
                          where method.GetGenericArguments().Length == 0 && method.GetParameters().Length == 0
                          select method;

            foreach (var method in methods)
                method.Invoke(null, CC.EmptyArguments);
        }

        private static void OnInstallResetFields(MemberInfo[] members)
        {
            var fields = from member in members
                         where member.MemberType == MemberTypes.Field
                         select (FieldInfo)member into field
                         where field.IsDefined<OnInstallResetableAttribute>(inherit: true)
                         select field;

            foreach (var field in fields)
            {
                if (field.FieldType.IsValueType)
                    field.SetValue(null, Activator.CreateInstance(field.FieldType));
                else
                    field.SetValue(null, null);
            }
        }

        private static bool OnInstallAssemblyFilter(Assembly assembly)
        {
            var assemblyName = assembly.GetName();

            if (assemblyName.Name.StartsWith("System"))
                return false;
            else if (assemblyName.Name.StartsWith("Microsoft"))
                return false;

            return true;
        }
    }
}
