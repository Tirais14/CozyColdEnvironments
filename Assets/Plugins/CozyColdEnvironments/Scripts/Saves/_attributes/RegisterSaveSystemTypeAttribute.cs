using System;
using CCEnvs.Attributes;

#nullable enable
namespace CCEnvs.Saves
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class RegisterSaveSystemTypeAttribute : Attribute, ICCAttribute
    {
        public Type SnapshotType { get; }

        public RegisterSaveSystemTypeAttribute(Type snapshotType)
        {
            SnapshotType = snapshotType;
        }
    }
}
