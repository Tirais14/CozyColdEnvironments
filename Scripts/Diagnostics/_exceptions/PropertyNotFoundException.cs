#nullable enable
using CCEnvs.Reflection;
using System;
using System.Reflection;

namespace CCEnvs.Diagnostics
{
    public class PropertyNotFoundException : MemberNotFoundException
    {
        public PropertyNotFoundException()
        {
        }

        public PropertyNotFoundException(Type reflectedType)
            :
            base(ReflectedType(reflectedType))
        {
        }

        public PropertyNotFoundException(Type reflectedType, string propertyName)
            :
            base($"{ReflectedType(reflectedType)}Property = {propertyName}")
        {
        }

        public PropertyNotFoundException(Type reflectedType, string propertyName, BindingFlags bindings)
            :
            base($"{ReflectedType(reflectedType)}Property = {propertyName}, bindings = {bindings}.")
        {
        }

        public PropertyNotFoundException(Type reflectedType,
                                         string propertyName,
                                         BindingFlags bindings,
                                         PropertyBindings propertyBindings)
            :
            base($"{ReflectedType(reflectedType)}Property = {propertyName}, bindings = {bindings}, property bindings = {propertyBindings}.")
        {
        }

        public PropertyNotFoundException(Type reflectedType, Type propertyType)
            :
            base($"{ReflectedType(reflectedType)}Property type = {propertyType.GetName()}.")
        {
        }

        public PropertyNotFoundException(Type reflectedType,
                                         Type propertyType,
                                         BindingFlags bindings)
            :
            base($"{ReflectedType(reflectedType)}Property type = {propertyType.GetName()}, bindings = {bindings}.")
        {
        }

        public PropertyNotFoundException(Type reflectedType,
                                         Type propertyType,
                                         BindingFlags bindings,
                                         PropertyBindings propertyBindings)
            :
            base($"{ReflectedType(reflectedType)}Property type = {propertyType.GetName()}, bindings = {bindings}, property bindings = {propertyBindings}.")
        {
        }
    }
}
