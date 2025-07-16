using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTIRLib.Attributes;
using UTIRLib.Diagnostics;
using UTIRLib.EnumFlags;
using UTIRLib.UExtensions;

#nullable enable

namespace UTIRLib.Injector
{
    public static class ComponentInjector
    {
        /// <summary>
        /// Finds and sets field and values marked with attribute <see cref="GetComponentAttribute"/>
        /// </summary>
        /// <param name="sourceComponent"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Inject(Component sourceComponent)
        {
            if (sourceComponent == null)
                throw new ArgumentNullException(nameof(sourceComponent));

            Type componentType = sourceComponent.GetType();
            object? targetComponent;
            if (TryGetMarkedMembers(componentType, out MemberInfo[] markedMembers))
            {
                foreach (var markedMember in markedMembers)
                {
                    targetComponent = GetTargetComponent(sourceComponent,
                                                         markedMember);

                    if (targetComponent.IsNotNull())
                        SetComponent(sourceComponent,
                                     markedMember,
                                     targetComponent);
                }
            }
        }

        private static object? GetTargetComponent(Component sourceComponent,
                                                  MemberInfo member)
        {
            GetComponentAttribute getComponentAttribute = member.GetCustomAttribute<GetComponentAttribute>();

            GetComponentSettings settings = IsOptionalMember(member) ? getComponentAttribute.Settings.ToOptional()
                : getComponentAttribute.Settings;

            if (settings == GetComponentSettings.None)
                throw new InvalidOperationException("Settings not setted.");

            object? memberValue;
            Type targetComponentType;
            switch (member)
            {
                case FieldInfo field:
                    memberValue = field.GetValue(sourceComponent);
                    targetComponentType = field.FieldType;
                    break;

                case PropertyInfo prop:
                    memberValue = prop.GetValue(sourceComponent);
                    targetComponentType = prop.PropertyType;
                    break;

                default: throw new InvalidOperationException();
            }

            if (!CheckMemberValue(sourceComponent,
                                  sourceComponent.GetTypeName(),
                                  member.Name,
                                  memberValue,
                                  settings)
                )
                return null;

            object? targetComponent;
            if (settings.IsFlagSetted(GetComponentSettings.GetAssignedObject))
                targetComponent = GetTargetAssignedObject(sourceComponent,
                                                          targetComponentType,
                                                          getComponentAttribute);
            else
                targetComponent = GetComponentFromSource(sourceComponent,
                                                         targetComponentType,
                                                         getComponentAttribute);

            if (!CheckTargetComponent(sourceComponent, targetComponentType, targetComponent, member.Name, settings))
                return null;

            return targetComponent;
        }

        private static bool IsOptionalMember(MemberInfo member)
        {
            return member.IsDefined(typeof(OptionalAttribute));
        }

        private static InvokeResult CheckMemberValue(Component sourceComponent,
                                                     string componentTypeName,
                                                     string memberName,
                                                     object? memberValue,
                                                     GetComponentSettings settings)
        {
            if (memberValue.IsNotNull())
            {
                string message = $"{componentTypeName} already setted component of field: {memberName}.";

                if (settings.IsFlagSetted(GetComponentSettings.ThrowIfSkipped))
                    throw new Exception(message);
                else if (settings.IsFlagSetted(GetComponentSettings.ErrorIfSkipped))
                    TirLibDebug.Error(message, sourceComponent);
                else if (settings.IsFlagSetted(GetComponentSettings.WarningIfSkipped))
                    TirLibDebug.Warning(message, sourceComponent);
                else if (settings.IsFlagSetted(GetComponentSettings.MessageIfSkipped))
                    TirLibDebug.Log(message, sourceComponent);

                return new InvokeResult(-1);
            }

            return new InvokeResult(0);
        }

        private static InvokeResult CheckTargetComponent(Component sourceComponent,
                                                         Type targetComponentType,
                                                         object? targetComponent,
                                                         string memberName,
                                                         GetComponentSettings settings)
        {
            if (targetComponent.IsNull())
            {
                ComponentNotFoundException exception = new(targetComponentType);

                if (settings.IsFlagSetted(GetComponentSettings.ThrowIfNotFound))
                    throw exception;
                else if (settings.IsFlagSetted(GetComponentSettings.ErrorIfNotFound))
                    TirLibDebug.Error(exception.Message, sourceComponent);
                else if (settings.IsFlagSetted(GetComponentSettings.WarningIfNotFound))
                    TirLibDebug.Warning(exception.Message, sourceComponent);
                else if (settings.IsFlagSetted(GetComponentSettings.MessageIfNotFound))
                    TirLibDebug.Log(exception.Message, sourceComponent);

                return new InvokeResult(-1);
            }

            return new InvokeResult(0);
        }

        private static Component? GetComponentFromSource(Component sourceComponent,
                                                         Type targetComponentType,
                                                         GetComponentAttribute attribute)
        {
            switch (attribute)
            {
                case GetComponentInChildrenAttribute attributeChild:
                    if (attributeChild.HasChildName &&
                        sourceComponent.transform.TryFind(attributeChild.ChildName!,
                                                          out Transform? child)
                        )
                        return child.GetComponent(targetComponentType);
                    else if (!attributeChild.HasChildName)
                        return sourceComponent.GetComponentInChildren(targetComponentType);

                    return null;

                case GetComponentInParentAttribute attributeParent:
                    if (attributeParent.HasParentName &&
                        sourceComponent.transform.TryFind(attributeParent.ParentName!,
                                                          out Transform? parent)
                        )
                        return parent.GetComponent(targetComponentType);
                    else if (!attributeParent.HasParentName)
                        return sourceComponent.GetComponentInParent(targetComponentType);

                    return null;

                default: 
                    return sourceComponent.GetComponent(targetComponentType);
            }
        }

        private static object? GetTargetAssignedObject(Component sourceComponent,
                                                       Type targetComponentType,
                                                       GetComponentAttribute attribute)
        {
            switch (attribute)
            {
                case GetComponentInChildrenAttribute attributeChild:
                    if (attributeChild.HasChildName
                        &&
                        sourceComponent.transform.TryFind(attributeChild.ChildName!,
                                                          out Transform? child)
                        )
                        return child.GetAssignedObject(targetComponentType);
                    else if (!attributeChild.HasChildName)
                        return sourceComponent.GetAssignedObjectInChildren(targetComponentType);

                    return null;

                case GetComponentInParentAttribute attributeParent:
                    if (attributeParent.HasParentName 
                        &&
                        sourceComponent.transform.TryFind(attributeParent.ParentName!,
                                                          out Transform? parent)
                        )
                        return parent.GetAssignedObject(targetComponentType);
                    else if (!attributeParent.HasParentName)
                        return sourceComponent.GetAssignedObjectInParent(targetComponentType);

                    return null;

                default: return sourceComponent.GetAssignedObject(targetComponentType);
            }
        }

        private static void SetComponent(Component sourceComponent,
                                         MemberInfo member,
                                         object targetComponent)
        {
            switch (member)
            {
                case FieldInfo field:
                    field.SetValue(sourceComponent, targetComponent);
                    break;

                case PropertyInfo prop:
                    prop.SetValue(sourceComponent, targetComponent);
                    break;

                default: throw new InvalidOperationException();
            }
        }

        private static bool TryGetMarkedMembers(Type type, out MemberInfo[] markedFields)
        {
            MemberInfo[] members = type.GetFields(BindingFlagsDefault.InstanceAll.ToBindingFlags())
                                       .Cast<MemberInfo>()
                                       .Concat(type.GetProperties(BindingFlagsDefault.InstanceAll.ToBindingFlags()).Cast<MemberInfo>())
                                       .ToArray();

            List<MemberInfo> results = new();
            int fieldsCount = members.Length;
            for (int i = 0; i < fieldsCount; i++)
            {
                if (members[i].IsDefined(typeof(GetComponentAttribute), inherit: true))
                    results.Add(members[i]);
            }

            markedFields = results.ToArray();
            return markedFields.Length > 0;
        }
    }
}