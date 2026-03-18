using CCEnvs.Attributes;
using System;

#nullable enable
namespace CCEnvs.Snapshots
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SnapshotPropertyAttribute : Attribute, ICCAttribute
    {
    }
}
