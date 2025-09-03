#nullable enable
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using System;
using System.Reflection;

namespace CCEnvs.Diagnostics
{
    public class MethodNotFoundException : MemberNotFoundException
    {
        public MethodNotFoundException()
        {
        }

        public MethodNotFoundException(Type reflectedType)
            :
            base(ReflectedType(reflectedType))
        {
        }

        public MethodNotFoundException(Type reflectedType, string methodName)
            :
            base($"{ReflectedType(reflectedType)}Method: {methodName}.")
        {
        }

        public MethodNotFoundException(Type reflectedType,
                                       string methodName,
                                       BindingFlags bindings)
            :
            base($"{ReflectedType(reflectedType)}Method: {methodName}, bindings = {bindings}.")
        {
        }

        public MethodNotFoundException(Type reflectedType,
                                       string methodName,
                                       BindingFlags bindings,
                                       CCParameters signature)
            :
            base($"{ReflectedType(reflectedType)}Method: {methodName}, bindings = {bindings}, signature = {signature}.")
        {
        }

        public MethodNotFoundException(Type reflectedType,
                                       string methodName,
                                       BindingFlags bindings,
                                       CCParameters signature,
                                       CCParameters genericParams)
            :
            base($"{ReflectedType(reflectedType)}Method: {methodName}, bindings = {bindings}, signature = {signature}, generic parameters = {genericParams}.")
        {
        }
    }
}
