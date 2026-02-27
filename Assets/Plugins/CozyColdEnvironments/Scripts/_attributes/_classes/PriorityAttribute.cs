using System;

#nullable enable
namespace CCEnvs.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class PriorityAttribute : Attribute, ICCAttribute
    {
        public int Priority { get; }

        public PriorityAttribute(int value)
        {
            Priority = value;
        }

        public PriorityAttribute()
            :
            this(default)
        {
        }
    }
}
