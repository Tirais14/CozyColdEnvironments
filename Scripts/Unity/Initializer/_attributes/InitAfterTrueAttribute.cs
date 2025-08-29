using System;
using CozyColdEnvironments.Reflection;
using CozyColdEnvironments.Diagnostics;

#nullable enable
namespace CozyColdEnvironments.Initables
{
    /// <summary>
    /// Not Implemented for now
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class InitAfterTrueAttribute : InitAttribute
    {
        public Type Type { get; }
        public string MemberName { get; }
        public MemberType MemberType { get; }
        public bool TypeDeclaredOnly { get; }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="StringArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public InitAfterTrueAttribute(Type type,
                                      string memberName,
                                      MemberType memberType,
                                      bool declaredOnly = false)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (memberName.IsNullOrWhiteSpace())
                throw new StringArgumentException(nameof(memberName), memberName);
            if (type.IsNotType<UnityEngine.Object>())
                throw new ArgumentException("Type must be UnityEngine.Object inherited.");
            if (memberType == MemberType.Undefined
                ||
                memberType == MemberType.Event
                ||
                memberType == MemberType.Constructor
                )
                throw new ArgumentException($"Member type cannot be {memberType}.");

            Type = type;
            MemberName = memberName;
            MemberType = memberType;
            TypeDeclaredOnly = declaredOnly;
        }
    }
}
