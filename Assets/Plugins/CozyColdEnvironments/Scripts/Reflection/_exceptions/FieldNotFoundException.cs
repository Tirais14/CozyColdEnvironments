#nullable enable
using CCEnvs.Reflection;
using System;
using System.Reflection;

namespace CCEnvs.Reflection
{
    public class FieldNotFoundException : MemberNotFoundException
    {
        public FieldNotFoundException()
        {
        }

        public FieldNotFoundException(Type reflectedType)
            :
            base(ReflectedType(reflectedType))
        {
        }

        public FieldNotFoundException(Type reflectedType,
                                      string fieldName,
                                      BindingFlags bindings)
            :
            base($"{ReflectedType(reflectedType)}Field = {fieldName}, bindings = {bindings}.")
        {
        }

        public FieldNotFoundException(Type reflectedType,
                                      Type fieldType)
            :
            base($"{ReflectedType(reflectedType)}Field type = {fieldType.GetName()}.")
        {
        }

        public FieldNotFoundException(Type reflectedType,
                                      Type fieldType,
                                      BindingFlags bindings)
            :
            base($"{ReflectedType(reflectedType)}Field type = {fieldType.GetName()}, bindings = {bindings}.")
        {
        }
    }
}
