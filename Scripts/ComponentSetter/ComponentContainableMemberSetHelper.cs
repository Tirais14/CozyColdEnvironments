using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection.Types;
using UTIRLib.UnityExtensions;

#nullable enable

namespace UTIRLib.ComponentSetter
{
    public static class ComponentContainableMemberSetHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static void SetMembers(Component target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            (FieldInfo field, GetComponentAttribute attribute)[] fields =
                GetAttributedFields(target);

            if (fields.IsNotEmpty())
                SetFields(target, fields);

            (PropertyInfo prop, GetComponentAttribute attribute)[] props =
                GetAttributedProps(target);

            if (props.IsNotEmpty())
                SetProps(target, props);
        }

        private static (FieldInfo, GetComponentAttribute)[]GetAttributedFields(
            Component source)
        {
            FieldInfo[] fields = source.GetType()
                                       .ForceGetFields(BindingFlagsDefault.InstanceAll);

            return fields.Where(x => x.IsDefined<GetComponentAttribute>())
                         .Select(x => (x, x.GetCustomAttribute<GetComponentAttribute>()))
                         .ToArray();
        }

        private static (PropertyInfo, GetComponentAttribute)[] GetAttributedProps(
            Component source)
        {
            PropertyInfo[] props = source.GetType()
                .ForceGetProperties(BindingFlagsDefault.InstanceAll);

            return props.Where(x => x.IsDefined<GetComponentAttribute>())
                        .Select(x => (x, x.GetCustomAttribute<GetComponentAttribute>()))
                        .ToArray();
        }

        private static object? SelfGetter(Component source, Type getType)
        {
            if (getType.Is<Component>())
                return source.GetComponent(getType);
            else
                return source.GetAssignedObject(getType);
        }

        private static object? ByParentGetter(Component source, Type getType)
        {
            if (getType.Is<Component>())
                return source.GetComponentInParent(getType);
            else
                return source.GetAssignedObjectInParent(getType);
        }

        private static object? ByChildrenGetter(Component source, Type getType)
        {
            if (getType.Is<Component>())
                return source.GetComponentInChildren(getType);
            else
                return source.GetAssignedObjectInChildren(getType);
        }

        private static bool IsTypeValid(Type type)
        {
            if (!type.IsInterface && type.IsNot<Component>())
                return false;

            return true;
        }

        /// <exception cref="ObjectNotFoundException"></exception>
        private static void SetField(Component source,
                                     FieldInfo field,
                                     Func<Component, Type, object?> getter)
        {
            if (field.GetValue(source).IsNotNull())
                return;
            if (!IsTypeValid(field.FieldType))
            {
                TirLibDebug.Error($"{field.FieldType.GetName()} is not interface and not component.", source, isExtraInfo: false);

                return;
            }

            object? foundComponent = getter(source, field.FieldType);

            if (foundComponent.IsNull())
                throw new ObjectNotFoundException(field.FieldType);

            field.SetValue(source, foundComponent);
        }

        private static void SetFields(Component source,
            (FieldInfo field, GetComponentAttribute attribute)[] fields)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                switch (fields[i].attribute)
                {
                    case GetBySelfAttribute:
                        SetField(source, fields[i].field, SelfGetter);
                        break;
                    case GetByParentAttribute:
                        SetField(source, fields[i].field, ByParentGetter);
                        break;
                    case GetByChildrenAttribute:
                        SetField(source, fields[i].field, ByChildrenGetter);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <exception cref="ObjectNotFoundException"></exception>
        private static void SetProp(Component source,
                                              PropertyInfo prop,
                                              Func<Component, Type, object?> getter)
        {
            if (prop.GetValue(source).IsNotNull())
                return;
            if (!IsTypeValid(prop.PropertyType))
            {
                TirLibDebug.Error($"{prop.PropertyType.GetName()} is not interface and not component.", source, isExtraInfo: false);

                return;
            }

            object? foundComponent;

            if (prop.PropertyType.Is<Component>())
                foundComponent = getter(source, prop.PropertyType);
            else
                foundComponent = getter(source, prop.PropertyType);

            if (foundComponent.IsNull())
                throw new ObjectNotFoundException(prop.PropertyType);

            prop.SetValue(source, foundComponent);
        }

        private static void SetProps(Component source,
            (PropertyInfo prop, GetComponentAttribute attribute)[] props)
        {
            for (int i = 0; i < props.Length; i++)
            {
                switch (props[i].attribute)
                {
                    case GetBySelfAttribute:
                        SetProp(source, props[i].prop, SelfGetter);
                        break;
                    case GetByParentAttribute:
                        SetProp(source, props[i].prop, ByParentGetter);
                        break;
                    case GetByChildrenAttribute:
                        SetProp(source, props[i].prop, ByChildrenGetter);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}