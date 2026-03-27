using CCEnvs.Attributes;
using System;

#nullable enable
namespace CCEnvs.Snapshots
{
    [AttributeUsage(
        AttributeTargets.Class
        | 
        AttributeTargets.Struct
        |
        AttributeTargets.Field
        |
        AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true
        )]
    public class SnapshotConvertibleAttribute : Attribute, ICCAttribute
    {
        public Type? SnapshotType { get; }

        public SnapshotConvertibleAttribute(Type? snapshotType = null)
        {
            SnapshotType = snapshotType;
        }
    }
}
