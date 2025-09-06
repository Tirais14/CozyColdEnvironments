#nullable enable
using System;

namespace CCEnvs.Attributes
{
    /// <summary>
    /// Marks static method as constructor for <see cref="InstanceFactory"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CCConstructorAttribute : Attribute, ICCAttribute
    {
        //public Type? TargetType { get; }
        //public bool HasTargetType => TargetType is not null;

        public CCConstructorAttribute()
        {
        }

        //public CCConstructorAttribute(Type targetType)
        //{
        //    TargetType = targetType;
        //}
    }
}
