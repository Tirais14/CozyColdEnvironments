using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;
using UTIRLib.Utils;
using Zenject;

#nullable enable

namespace UTIRLib.Zenject
{
    public static class DiContainerHelper
    {
        public static bool HasInjectableComponent(GameObject gameObject) =>
            HasInjectableComponentInternal(gameObject, saveResults: false, out _);

        public static bool HasInjectableComponent(GameObject gameObject, out Component[] injectableComponents) =>
            HasInjectableComponentInternal(gameObject, saveResults: false, out injectableComponents);

        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsInjectable(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            return HasInjectAttribute<MethodInfo>(type, bindingFlags) || HasInjectAttribute<PropertyInfo>(type, bindingFlags) ||
                HasInjectAttribute<FieldInfo>(type, bindingFlags);
        }

        public static bool IsInjectable<T>() => IsInjectable(typeof(T));

        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsInjectable(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return IsInjectable(obj.GetType());
        }

        public static bool IsInjectable(GameObject gameObject) => gameObject.GetComponents<Component>().Any(IsInjectable);

        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasInjectAttribute<TMember>(TMember member)
            where TMember : MemberInfo
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            return member.IsDefined(typeof(InjectAttribute), inherit: true);
        }

        public static bool HasInjectAttribute<TMember>(TMember[]? members)
            where TMember : MemberInfo
        {
            if (members.IsNullOrEmpty())
            {
                return false;
            }

            int membersCount = members.Length;
            for (int i = 0; i < membersCount; i++)
            {
                if (HasInjectAttribute(members[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static bool HasInjectAttribute<TMember>(Type type, BindingFlags bindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where TMember : MemberInfo
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            TMember[] members = TypeHelper.GetAllMembers<TMember>(type, bindingFlags);
            return HasInjectAttribute(members);
        }

        ///// <exception cref="Exception"></exception>
        //public static void InjectDependencies(object target, IEnumerable<DiContainer> containers)
        //{
        //    List<object> dependencies = new();
        //    if (TryResolveDependencies<MemberInfo>(target, containers, dependencies)) {
        //        MemberInfo[] injectableMembers = GetInjectableMembers<MemberInfo>(target.GetType());
        //        for (int i = 0; i < injectableMembers.Length; i++) {
        //            TypeHelper.ResolveArguments(injectableMembers[i], target, out _, dependencies);
        //        }
        //    }
        //    else throw new Exception($"Cannot resolve dependencies for {target.GetTypeName()}.");
        //}

        #region Resolvers

        public static bool TryResolveDependencies<TMember>(TMember member, DiContainer container, IList<object> dependencies)
            where TMember : MemberInfo
        {
            ValidateMethodResolveDependencies(container, member, dependencies).TryThrow();
            if (member is MethodInfo method)
            {
                ParameterInfo[] parameters = method.GetParameters();
                int resolvedCount = 0;
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (container.HasBinding(parameters[i].ParameterType))
                    {
                        dependencies.Add(container.Resolve(parameters[i].ParameterType));
                        resolvedCount++;
                    }
                }

                return parameters.Length == resolvedCount;
            }
            else if (member is PropertyInfo property && container.HasBinding(property.PropertyType))
            {
                dependencies.Add(container.Resolve(property.PropertyType));
                return true;
            }
            else if (member is FieldInfo field && container.HasBinding(field.FieldType))
            {
                dependencies.Add(container.Resolve(field.FieldType));
                return true;
            }

            return false;
        }

        public static bool TryResolveDependencies<TMember>(TMember[] members, IEnumerable<DiContainer> containers,
            IList<object> dependencies)
            where TMember : MemberInfo
        {
            int dependenciesPreviousCount = dependencies.Count;
            for (int i = 0; i < members.Length; i++)
            {
                foreach (DiContainer container in containers)
                {
                    if (TryResolveDependencies(members[i], container, dependencies))
                    {
                        break;
                    }
                }
            }

            return dependencies.Count > dependenciesPreviousCount;
        }

        public static bool TryResolveDependencies<TMember>(Type type, IEnumerable<DiContainer> containers,
            IList<object> dependencies)
            where TMember : MemberInfo => TryResolveDependencies(GetInjectableMembers<TMember>(type), containers, dependencies);

        public static bool TryResolveDependencies<TMember>(object target, IEnumerable<DiContainer> containers,
            IList<object> dependencies)
            where TMember : MemberInfo => TryResolveDependencies<TMember>(target.GetType(), containers, dependencies);

        /// <exception cref="UnableToResolveException"></exception>
        public static void ResolveDependencies<TMember>(TMember[] members, IEnumerable<DiContainer> containers,
            IList<object> dependencies)
            where TMember : MemberInfo
        {
            if (!TryResolveDependencies(members, containers, dependencies))
            {
                throw new UnableToResolveException();
            }
        }

        /// <exception cref="UnableToResolveException"></exception>
        public static void ResolveDependencies(Type targetType, IEnumerable<DiContainer> containers, IList<object> dependencies)
        {
            MemberInfo[] injectableMembers = GetInjectableMembers<MemberInfo>(targetType);
            if (!TryResolveDependencies(injectableMembers, containers, dependencies))
            {
                throw new UnableToResolveException(targetType.Name);
            }
        }

        public static void ResolveDependencies(object target, IEnumerable<DiContainer> containers, IList<object> dependencies) =>
            ResolveDependencies(target.GetType(), containers, dependencies);

        /// <exception cref="UnableToResolveException"></exception>
        public static void ResolveDependencies<TMember>(TMember member, DiContainer container, IList<object> dependencies)
            where TMember : MemberInfo
        {
            if (!TryResolveDependencies(member, container, dependencies))
            {
                throw new UnableToResolveException(member);
            }
        }

        #endregion Resolvers

        public static TMember[] GetInjectableMembers<TMember>(Type targetType, BindingFlags bindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where TMember : MemberInfo
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            MemberInfo[] allMembers = targetType.GetMembers(bindingFlags) ?? Array.Empty<MemberInfo>();
            List<TMember> results = new();
            for (int i = 0; i < allMembers.Length; i++)
            {
                if (HasInjectAttribute(allMembers[i]) && allMembers[i] is TMember typedMember)
                {
                    results.Add(typedMember);
                }
            }

            return results.ToArray();
        }

        private static bool HasInjectableComponentInternal(GameObject gameObject, bool saveResults,
            out Component[] injectableComponents)
        {
            injectableComponents = Array.Empty<Component>();
            if (gameObject.IsNull())
            {
                throw new ArgumentNullException(nameof(gameObject));
            }
            bool result = false;

            var components = gameObject.GetComponents(typeof(Component));
            List<Component>? injectableComponentList = null;
            bool listCreated = false;
            for (int i = 0; i < components.Length; i++)
            {
                if (IsInjectable(components[i]))
                {
                    result = true;
                    if (saveResults)
                    {
                        if (!listCreated)
                        {
                            injectableComponentList = new List<Component>();
                            listCreated = true;
                        }
                        injectableComponentList ??= new List<Component>();
                        injectableComponentList.Add(components[i]);
                    }
                    else break;
                }
            }

            injectableComponents = injectableComponentList != null ? injectableComponentList.ToArray() :
                Array.Empty<Component>();
            return result;
        }

        private static ValidationInfo ValidateMethodResolveDependencies(DiContainer container, MemberInfo member,
            IList<object> dependencies)
        {
            if (container == null)
            {
                return new ValidationInfo(new ArgumentNullException(nameof(container)));
            }
            if (member == null)
            {
                return new ValidationInfo(new ArgumentNullException(nameof(member)));
            }
            if (dependencies == null)
            {
                return new ValidationInfo(new ArgumentNullException(nameof(dependencies)));
            }

            return default;
        }
    }
}