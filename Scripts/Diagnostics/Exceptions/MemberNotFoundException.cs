using System;
using System.Reflection;
using System.Security.Cryptography;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.Diagnostics
{
    public class MemberNotFoundException : TirLibException
    {
        public MemberNotFoundException()
        {
        }

        public MemberNotFoundException(Type targetType) : base($"Target Type: {targetType.Name}.")
        {
        }

        public MemberNotFoundException(Type targetType, MemberType memberType)
            :
            base($"Reflected type = {targetType.GetName()}, member type = {memberType}.")
        {
        }

        public MemberNotFoundException(Type targetType,
                                       MemberType memberType,
                                       string memberName)
            :
            base($"Reflected type = {targetType.GetName()}, member type = {memberType}, member name = {memberName}.")
        {
        }

        public MemberNotFoundException(Type targetType,
                                       MemberType memberType,
                                       TypeMemberParameters parameters)
            :
            base($"Reflected type = {targetType.GetName()}, member type = {memberType}, parameters = {parameters}.")
        {
        }

        public MemberNotFoundException(Type targetType,
                                       MemberType memberType,
                                       TypeMemberParameters parameters,
                                       string message)
            :
            base($"Reflected type = {targetType.GetName()}, member type = {memberType}, parameters = {parameters}. {message.TrimEnd('.')}.")
        {
        }
    }
}
