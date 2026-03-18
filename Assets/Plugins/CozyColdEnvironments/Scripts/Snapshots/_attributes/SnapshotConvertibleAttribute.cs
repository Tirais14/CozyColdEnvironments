using CCEnvs.Attributes;
using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Snapshots
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class SnapshotConvertibleAttribute : Attribute, ICCAttribute
    {
        public Type? SnapshotType { get; }

        public Type[] CompositeParts { get; }

        public SnapshotConvertibleAttribute(
            Type? snapshotType, 
            params Type[] compositeParts
            )
        {
            Guard.IsNotNull(compositeParts, nameof(compositeParts));

            SnapshotType = snapshotType;
            CompositeParts = compositeParts;
        }
    }
}
