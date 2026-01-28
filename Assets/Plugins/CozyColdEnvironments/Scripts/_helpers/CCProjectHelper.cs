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
                var assemblyTypes =
                    (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     where OnInstallAssemblyFilter(assembly)
                     select assembly.GetTypes() into tps
                     from type in tps
                     where OnInstallTypeFilter(type)
                     select type
                     )
                     .ToArray();

                IEnumerable<Type> types = assemblyTypes;

                foreach (var assemblyType in assemblyTypes)
                {
                    types = types.Concat(Loops.BreadthFirstSearch(assemblyType,
                        static assemblyType =>
                        {
                            return assemblyType.GetNestedTypes(BindingFlagsDefault.All);
                        })
                        .Skip(1)); //Excludes assemblyType from collection
                }

                var members = GetMembers(BindingFlagsDefault.StaticAll, types.ToArray());

                OnInstallProcessFields(null, members);
                OnInstallExecuteMethods(null, members);
            }
            finally
            {
                IsInstalling = false;
            }
        }

        private static IEnumerable<MemberInfo> GetMembers(BindingFlags bindingFlags, params Type[] types)
        {
            return types.SelectMany(type => type.GetMembers(bindingFlags));
        }

        private static void OnInstallExecuteMethods(object? target, IEnumerable<MemberInfo> members)
        {
            var methods = from member in members
                          where member.MemberType == MemberTypes.Method
                          select (MethodInfo)member into method
                          where method.IsDefined<OnInstallMethodAttribute>(inherit: true)
                          where method.GetGenericArguments().Length == 0
                                &&
                                method.GetParameters().Count(param => !param.HasDefaultValue) == 0

                          select method;

            foreach (var method in methods)
            {
                try
                {
                    method.Invoke(target, CC.EmptyArguments);
                }
                catch (Exception ex)
                {
                    typeof(CCProjectHelper).PrintException(ex);
                }
            }
        }

        private static void OnInstallProcessFields(
            object? target,
            IEnumerable<MemberInfo> members)
        {
            var fieldInfos = (from member in members
                              where member.MemberType == MemberTypes.Field
                              select (FieldInfo)member into field
                              where !field.FieldType.ContainsGenericParameters
                              where field.IsDefined<OnInstallAttribute>(inherit: true)
                              select field)
                              .ToArray();

            OnInstallResetFields(target, fieldInfos);
            OnInstallExecuteFieldMethods(fieldInfos);
        }

        private static void OnInstallResetFields(
            object? target,
            IEnumerable<FieldInfo> fields
            )
        {
            Reflect fieldReflect;

            foreach (var (field, fieldValue) in

                from field in fields
                where field.IsDefined<OnInstallResetableAttribute>(inherit: true)
                select (field, fieldValue: field.GetValue(target)) into fieldInfo
                where fieldInfo.fieldValue.IsNotDefault()
                select fieldInfo
                )
            {
                try
                {
                    fieldReflect = new Reflect();

                    fieldReflect.From(field.FieldType)
                        .IncludeInstance()
                        .IncludeNonPublic()
                        .WithName("Clear")
                        .Method()
                        .Lax()
                        .IfSome(method => method.Invoke(fieldValue, CC.EmptyArguments));

                    fieldReflect.From(field.FieldType)
                        .IncludeInstance()
                        .IncludeNonPublic()
                        .WithName("Reset")
                        .Method()
                        .Lax()
                        .IfSome(method => method.Invoke(fieldValue, CC.EmptyArguments));

                    if (field.IsInitOnly)
                        continue;

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
            IEnumerable<FieldInfo> fields
            )
        {
            foreach (var (field, fieldValue) in

                from field in fields
                select (field, fieldValue: field.GetValue(null)) into fieldInfo
                where fieldInfo.fieldValue.IsNotDefault()
                select fieldInfo
                )
            {
                try
                {
                    var members = GetMembers(BindingFlagsDefault.InstanceAll, field.FieldType);
                    OnInstallExecuteMethods(fieldValue, members);
                }
                catch (Exception ex)
                {
                    typeof(CCProjectHelper).PrintException(ex);
                }
            }
        }

        private static string[] GetOnInstallFilters()
        {
            return new string[]
            {
                "System",
                "Microsoft",
                "Unity",
                "UniTask",
                "Cysharp",
                "mscorlib",
                "Mono",
                "R3",
                "MessagePipe",
                "Newtonsoft",
                "Codice.",
                "DG.",
                "Zenject"
            };
        }

        private static bool OnInstallAssemblyFilter(Assembly assembly)
        {
            var assemblyName = assembly.GetName();

            if (assemblyName is null)
                return false;

            return !GetOnInstallFilters().Any(filter =>
            {
                var t = assemblyName.Name.StartsWith(filter);

                return t;
            });
        }

        private static bool OnInstallTypeFilter(Type type)
        {
            if (type.Namespace is null)
                return true;

            return !GetOnInstallFilters().Any(filter =>
            {
                var t = type.Namespace.StartsWith(filter);

                return t;
            });
        }

        //private static void OnInstallProcessProperties(
        //    MemberInfo[] members
        //    )
        //{
        //    var props = from member in members
        //                where member.MemberType == MemberTypes.Property
        //                where member.IsDefined<OnInstallAttribute>()
        //                select ((PropertyInfo)member).

        //}
    }
}
