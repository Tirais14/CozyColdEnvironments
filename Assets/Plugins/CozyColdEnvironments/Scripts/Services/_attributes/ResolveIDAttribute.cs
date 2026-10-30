using System;

#nullable enable
namespace CCEnvs.Services
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ResolveIDAttribute : Attribute
    {
        public object ID { get; }

        public ResolveIDAttribute(object id)
        {
            CC.Guard.IsNotNull(id, nameof(id));

            ID = id;
        }
    }
}
