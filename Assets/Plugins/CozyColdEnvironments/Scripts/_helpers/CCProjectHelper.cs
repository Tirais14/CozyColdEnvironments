#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Reflection;
using SuperLinq;

namespace CCEnvs
{
    public static class CCProjectHelper
    {
        public static bool IsInstalling { get; private set; }

        public static MemberInfo[] GetDomainMembers(
            MemberTypes memberTypes,
            string[]? additionalAssemblyNames = null
            )
        {
            memberTypes &= ~MemberTypes.NestedType;

            var assemblyNames = GetDefaultAssemnlyNames().ConcatToArray(additionalAssemblyNames ?? new arr<string>());

            var assemblyNamesPartial = processAssemblyNames(assemblyNames);

            return (from assembly in AppDomain.CurrentDomain.GetAssemblies().AsParallel()
                    where assemblyFilter(assembly)
                    select assembly.GetTypes() into types
                    from type in types
                    select Loops.BreadthFirstSearch(type, type => type.GetNestedTypes()) into types
                    from type in types
                    select type.FindMembers(memberTypes, BindingFlagsDefault.All, (_, _) => true, null).Prepend(type) into members
                    from member in members
                    select member
                    )
                    .Distinct()
                    .ToArray();

            static bool[] processAssemblyNames(string[] assemblyNames)
            {
                var assemblyNamesPartial = new bool[assemblyNames.Length];

                string assemblyName;

                for (int i = 0; i < assemblyNames.Length; i++)
                {
                    assemblyName = assemblyNames[i];

                    assemblyNamesPartial[i] = assemblyName.EndsWith('*');

                    assemblyNames[i] = assemblyName.TrimEnd('*');
                }

                return assemblyNamesPartial;
            }

            bool assemblyFilter(Assembly assembly)
            {
                string assemblyName;

                for (int i = 0; i < assemblyNames.Length; i++)
                {
                    assemblyName = assemblyNames[i];

                    if (assemblyNamesPartial[i]
                        &&
                        assembly.FullName.Contains(assemblyName))
                    {
                        return true;
                    }
                    else if (assembly.FullName == assemblyName)
                        return true;
                }

                return false;
            }
        }

        public static void Install(MemberInfo[] domainMembers)
        {
            if (IsInstalling)
                throw new InvalidOperationException("Installing process already started");

            IsInstalling = true;

            try
            {
                var types = domainMembers.AsParallel().OfType<Type>().ToArray();

                var members = GetMembers(BindingFlagsDefault.StaticAll, types);

                OnInstallProcessFields(null, members);
                OnInstallExecuteMethods(null, members, domainMembers);
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

        private static void OnInstallExecuteMethods(
            object? target,
            IEnumerable<MemberInfo> members,
            MemberInfo[]? domainMembers
            )
        {
            var methodInfos =
                from member in members
                where member.MemberType == MemberTypes.Method
                select (MethodInfo)member into method
                where method.IsDefined<OnInstallExecutableAttribute>(inherit: true)
                select (method, prms: method.GetParameters(), genericArgs: method.GetGenericArguments()) into methodInfo
                where methodInfo.genericArgs.IsNullOrEmpty()
                where methodInfo.prms.IsNullOrEmpty()
                      ||
                      (methodInfo.prms.Length == 1
                      &&
                      methodInfo.prms[0].ParameterType.IsType<IEnumerable<MemberInfo>>())

                select methodInfo;

            foreach (var (method, prms, _) in methodInfos)
            {
                try
                {
                    if (prms.IsNotNullOrEmpty() && domainMembers.IsNotNullOrEmpty())
                    {
                        method.Invoke(target, new object[] { domainMembers });
                        continue;
                    }

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
                        .WithName("Clear")
                        .Method()
                        .Lax()
                        .IfSome(method =>
                        {
                            method.Invoke(fieldValue, CC.EmptyArguments);
                        });

                    fieldReflect.From(field.FieldType)
                        .IncludeInstance()
                        .WithName("Reset")
                        .Method()
                        .Lax()
                        .IfSome(method =>
                        {
                            method.Invoke(fieldValue, CC.EmptyArguments);
                        });

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
                    OnInstallExecuteMethods(fieldValue, members, null);
                }
                catch (Exception ex)
                {
                    typeof(CCProjectHelper).PrintException(ex);
                }
            }
        }

        private static string[] GetDefaultAssemnlyNames()
        {
            return new string[]
            {
                "CCEnvs*"
            };
        }
    }
}
