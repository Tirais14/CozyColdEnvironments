using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.UExtensions;

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

            (FieldInfo field, ComponentContainableAttribute attribute)[] fields =
                GetAttributedFields(target);

            if (fields.IsNotEmpty())
                SetFields(target, fields);

            (PropertyInfo prop, ComponentContainableAttribute attribute)[] props =
                GetAttributedProps(target);

            if (props.IsNotEmpty())
                SetProps(target, props);
        }

        private static (FieldInfo, ComponentContainableAttribute)[]GetAttributedFields(
            Component source)
        {
            return source.GetType()
                         .GetFields(BindingFlagsDefault.InstanceAll.ToBindingFlags())
                         .Where(x => x.IsDefined<ComponentContainableAttribute>())
                         .Select(x => (x, x.GetCustomAttribute<ComponentContainableAttribute>()))
                         .ToArray();
        }

        private static (PropertyInfo, ComponentContainableAttribute)[] GetAttributedProps(
            Component source)
        {
            return source.GetType()
                         .GetProperties(BindingFlagsDefault.InstanceAll.ToBindingFlags())
                         .Where(x => x.IsDefined<ComponentContainableAttribute>())
                         .Select(x => (x, x.GetCustomAttribute<ComponentContainableAttribute>()))
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

        /// <exception cref="ObjectNotFoundException"></exception>
        private static void SetField(Component source,
                                     FieldInfo field,
                                     Func<Component, Type, object?> getter)
        {
            object? foundComponent = getter(source, field.FieldType);

            if (foundComponent.IsNull())
                throw new ObjectNotFoundException(field.FieldType);

            field.SetValue(source, foundComponent);

            return;
        }

        private static void SetFields(Component source,
            (FieldInfo field, ComponentContainableAttribute attribute)[] fields)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                switch (fields[i].attribute)
                {
                    case GetSelfAttribute:
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
            object? foundComponent;

            if (prop.PropertyType.Is<Component>())
                foundComponent = source.GetComponent(prop.PropertyType);
            else
                foundComponent = source.GetAssignedObject(prop.PropertyType);

            if (foundComponent.IsNull())
                throw new ObjectNotFoundException(prop.PropertyType);

            prop.SetValue(source, foundComponent);

            return;
        }

        private static void SetProps(Component source,
            (PropertyInfo prop, ComponentContainableAttribute attribute)[] props)
        {
            for (int i = 0; i < props.Length; i++)
            {
                switch (props[i].attribute)
                {
                    case GetSelfAttribute:
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