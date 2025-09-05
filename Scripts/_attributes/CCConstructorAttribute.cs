#nullable enable
using System;

namespace CCEnvs.Attributes
{
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
