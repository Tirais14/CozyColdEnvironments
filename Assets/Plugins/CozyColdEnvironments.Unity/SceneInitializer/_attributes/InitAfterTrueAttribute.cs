using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Unity.Initables
{
    /// <summary>
    /// Not Implemented for now
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class InitAfterTrueAttribute : InitAttribute
    {
        public Type Type { get; }
        public string MemberName { get; }
        public MemberTypes MemberType { get; }
        public bool TypeDeclaredOnly { get; }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="EmptyStringArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public InitAfterTrueAttribute(Type type,
                                      string memberName,
                                      MemberTypes memberType,
                                      bool declaredOnly = false)
        {
            Guard.IsNotNull(type, nameof(type));
            Guard.IsNotNullOrWhiteSpace(memberName, nameof(memberName));
            CC.Guard.IsNotType<UnityEngine.Object>(type, nameof(type));
            if (memberType == MemberTypes.Custom
                ||
                memberType == MemberTypes.Event
                ||
                memberType == MemberTypes.Constructor
                )
            {
                throw new ArgumentException($"Member type cannot be {memberType}.");
            }

            Type = type;
            MemberName = memberName;
            MemberType = memberType;
            TypeDeclaredOnly = declaredOnly;
        }
    }
}
