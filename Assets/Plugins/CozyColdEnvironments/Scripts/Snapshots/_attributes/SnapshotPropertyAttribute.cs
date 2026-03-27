using CCEnvs.Attributes;
using CCEnvs.Reflection;
using System;

#nullable enable
namespace CCEnvs.Snapshots
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SnapshotPropertyAttribute : MemberIDAttribute, ICCAttribute
    {
        public SnapshotPropertyAttribute(string id)
            :
            base(id)
        {
        }
    }
}
