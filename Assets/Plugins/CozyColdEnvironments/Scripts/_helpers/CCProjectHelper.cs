#nullable enable
using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static void Install()
        {
            if (IsInstalling)
                throw new InvalidOperationException("Installing process already started");

            IsInstalling = true;

            try
            {
                var types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             where OnInstallAssemblyFilter(assembly)
                             select assembly.GetTypes() into tps
                             from type in tps
                             select type)
                             .ToArray();

                var members = GetMembersInfos(BindingFlagsDefault.StaticAll, types);

                OnInstallExecuteMethods(null, members);
                OnInstallProcessFields(null, members);
            }
            finally
            {
                IsInstalling = false;
            }
        }

        private static (MemberInfo member, Dictionary<Type, Attribute> attributes)[] GetMembersInfos(
            BindingFlags bindingFlags,
            params Type[] types)
        {
            return (from type in types
                    select type.GetMembers(bindingFlags) into mbms
                    from member in mbms
                    select (member, attributes: member.GetCustomAttributes(inherit: true)
                    .Cast<Attribute>()
                    .Select(att => (value: att, type: att.GetType()))
                    .DistinctBy(attInfo => attInfo.type)
                    .Select(attInfo => attInfo.value)
                    .ToDictionary(attInfo => attInfo.GetType()))
                    )
                    .ToArray();
        }

        private static void OnInstallExecuteMethods(object? target, (MemberInfo member, Dictionary<Type, Attribute> attributes)[] memberInfos)
        {
            var methodInfos = from memberInfo in memberInfos
                          where memberInfo.member.MemberType == MemberTypes.Method
                          select (method: (MethodInfo)memberInfo.member, memberInfo.attributes) into methodInfo
                          where methodInfo.attributes.ContainsKey(typeof(OnInstallMethodAttribute))
                          where methodInfo.method.GetGenericArguments().Length == 0
                                &&
                                methodInfo.method.GetParameters().Where(param => !param.HasDefaultValue).Count() == 0

                          select methodInfo;

            foreach (var methodInfo in methodInfos)
            {
                try
                {
                    methodInfo.method.Invoke(target, CC.EmptyArguments);
                }
                catch (Exception ex)
                {
                    typeof(CCProjectHelper).PrintException(ex);
                }
            }
        }

        private static void OnInstallProcessFields(
            object? target,
            (MemberInfo member, Dictionary<Type, Attribute> attributes)[] memberInfos)
        {
            var fieldInfos = (from memberInfo in memberInfos
                              where memberInfo.member.MemberType == MemberTypes.Field
                              select (field: (FieldInfo)memberInfo.member, memberInfo.attributes) into fieldInfo
                              where fieldInfo.attributes.ContainsKey(typeof(OnInstallMethodAttribute))
                                    ||
                                    fieldInfo.attributes.ContainsKey(typeof(OnInstallResetableAttribute))

                              select fieldInfo)
                              .ToArray();

            OnInstallResetFields(target, fieldInfos);
            OnInstallExecuteFieldMethods(fieldInfos);
        }

        private static void OnInstallResetFields(
            object? target,
            (FieldInfo field, Dictionary<Type, Attribute> attributes)[] fieldInfos
            )
        {
            foreach (var (field, attributes, fieldValue) in

                from fieldInfo in fieldInfos
                where !fieldInfo.field.IsInitOnly
                where fieldInfo.attributes.ContainsKey(typeof(OnInstallResetableAttribute))
                select (fieldInfo.field, fieldInfo.attributes, fieldValue: fieldInfo.field.GetValue(target)) into fieldInfo
                where fieldInfo.fieldValue.IsNotDefault()
                select fieldInfo
                )
            {
                try
                {
                    if (fieldValue is IDisposable disposable)
                        disposable.Dispose();

                    if (field.FieldType.IsValueType)
                        field.SetValue(target, Activator.CreateInstance(field.FieldType));
                    else
                        field.SetValue(target, null);
                }
                catch (Exception ex)
                {
                    typeof(CCProjectHelper).PrintException(ex);
                }
            }
        }

        private static void OnInstallExecuteFieldMethods(
            (FieldInfo field, Dictionary<Type, Attribute> attributes)[] fieldInfos
            )
        {
            foreach (var (field, attributes, fieldValue) in

                from fieldInfo in fieldInfos
                where fieldInfo.attributes.ContainsKey(typeof(OnInstallMethodAttribute))
                select (fieldInfo.field, fieldInfo.attributes, fieldValue: fieldInfo.field.GetValue(null)) into fieldInfo
                where fieldInfo.fieldValue.IsNotDefault()
                select fieldInfo
                )
            {
                try
                {
                    var memberInfos = GetMembersInfos(BindingFlagsDefault.InstanceAll, field.FieldType);
                    OnInstallExecuteMethods(fieldValue, memberInfos);
                }
                catch (Exception ex)
                {
                    typeof(CCProjectHelper).PrintException(ex);
                }
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
