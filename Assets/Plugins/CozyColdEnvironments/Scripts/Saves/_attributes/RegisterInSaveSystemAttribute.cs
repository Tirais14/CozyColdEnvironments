using CCEnvs.Attributes;
using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Saves
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class RegisterInSaveSystemAttribute : Attribute, ICCAttribute
    {
        public Type? SnapshotType { get; }

        public Type[] CompositerParts { get; } = Type.EmptyTypes;

        public RegisterInSaveSystemAttribute(Type snapshotType)
        {
            Guard.IsNotNull(snapshotType, nameof(snapshotType));

            SnapshotType = snapshotType;
        }

        public RegisterInSaveSystemAttribute(
            Type? snapshotType,
            params Type[] compositeParts
            )
        {
            Guard.IsNotNull(compositeParts, nameof(compositeParts));

            SnapshotType = snapshotType;
            CompositerParts = compositeParts ?? Type.EmptyTypes;
        }
    }
}
