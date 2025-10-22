using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Unity.Diagnostics;
using CCEnvs.Unity.Extensions;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;
using CCEnvs.Unity.Components;

#nullable enable

namespace CCEnvs.Unity.Injections
{
    /// <summary>
    /// Collect all components by <see cref="GetComponentAttribute"/> and setts to it fields or properties.
    /// <see cref="CCBehaviour"/> already contains implements this.
    /// </summary>
    public static class ComponentInjector
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static void Inject(Component target)
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

        private static (FieldInfo, GetComponentAttribute)[] GetAttributedFields(
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

        private static object? SelfGetter(Component source,
                                          Type getType,
                                          GetComponentAttribute _)
        {
            if (getType.IsType<Component>())
                return source.GetComponent(getType);
            else
                return source.GetAssignedObject(getType);
        }

        private static object? ByParentGetter(Component source,
                                              Type getType,
                                              GetComponentAttribute _)
        {
            if (getType.IsType<Component>())
                return source.GetComponentInParent(getType);
            else
                return source.GetAssignedObjectInParent(getType);
        }

        private static object? ByChildrenGameObjectGetter(Component source,
                                                          Type getType,
                                                          GetByChildrenAttribute attribute)
        {
            if (getType.IsType<Component>())
            {
                GameObject? go = source.gameObject.Find(attribute.GameObejctName!);

                if (go.IsNull())
                    throw new GameObjectNotFoundException(typeof(GameObject));

                return go.GetComponent(getType);
            }
            else
            {
                GameObject? go = source.gameObject.Find(attribute.GameObejctName!);

                if (go.IsNull())
                    throw new GameObjectNotFoundException(typeof(GameObject));

                return go.GetAssignedObject(getType);
            }
        }

        private static object? ByChildrenGetter(Component source,
                                                Type getType,
                                                GetComponentAttribute attribute)
        {
            var typedAttribute = (GetByChildrenAttribute)attribute;
            if (typedAttribute.HasGameObjectName)
                return ByChildrenGameObjectGetter(source, getType, typedAttribute);

            if (getType.IsType<Component>())
                return source.GetComponentInChildren(getType);
            else
                return source.GetAssignedObjectInChildren(getType);
        }

        private static bool IsTypeValid(Type type)
        {
            if (!type.IsInterface && type.IsNotType<Component>())
                return false;

            return true;
        }

        /// <exception cref="GameObjectNotFoundException"></exception>
        private static void SetField(Component source,
            FieldInfo field,
            GetComponentAttribute attribute,
            Func<Component, Type, GetComponentAttribute, object?> getter)
        {
            if (field.GetValue(source).IsNotNull())
            {
                CCDebug.PrintLog($"Field {field.FieldType.GetName()} is {field.ReflectedType.GetName()} already setted.");
                return;
            }
            if (!IsTypeValid(field.FieldType))
            {
                CCDebug.PrintError($"{field.FieldType.GetName()} is not interface and not component.", source);
                return;
            }

            object? foundComponent = getter(source, field.FieldType, attribute);

            if (foundComponent.IsNull())
                throw new GameObjectNotFoundException(field.FieldType);

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
                        SetField(source, fields[i].field, attribute: null!, SelfGetter);
                        break;
                    case GetByParentAttribute:
                        SetField(source, fields[i].field, attribute: null!, ByParentGetter);
                        break;
                    case GetByChildrenAttribute:
                        SetField(source, fields[i].field, fields[i].attribute, ByChildrenGetter);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <exception cref="GameObjectNotFoundException"></exception>
        private static void SetProp(Component source,
            PropertyInfo prop,
            GetComponentAttribute attribute,
            Func<Component, Type, GetComponentAttribute, object?> getter)
        {
            if (prop.GetValue(source).IsNotNull())
                return;
            if (!IsTypeValid(prop.PropertyType))
            {
                CCDebug.PrintError($"{prop.PropertyType.GetName()} is not interface and not component.", source);
                return;
            }

            object? foundComponent;

            foundComponent = getter(source, prop.PropertyType, attribute);

            if (foundComponent.IsNull())
                throw new GameObjectNotFoundException(prop.PropertyType);

            prop.SetValue(source,
                          foundComponent,
                          BindingFlagsDefault.InstanceAll,
                          binder: null,
                          index: null,
                          culture: CultureInfo.InvariantCulture);
        }

        private static void SetProps(Component source,
            (PropertyInfo prop, GetComponentAttribute attribute)[] props)
        {
            for (int i = 0; i < props.Length; i++)
            {
                switch (props[i].attribute)
                {
                    case GetBySelfAttribute:
                        SetProp(source, props[i].prop, attribute: null!, SelfGetter);
                        break;
                    case GetByParentAttribute:
                        SetProp(source, props[i].prop, attribute: null!, ByParentGetter);
                        break;
                    case GetByChildrenAttribute:
                        SetProp(source, props[i].prop, props[i].attribute, ByChildrenGetter);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}