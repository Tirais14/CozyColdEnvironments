using System;

#nullable enable
namespace CCEnvs.Unity.AddrsAssets
{
    /// <summary>
    /// Marks data type for <see cref="AddressablesDatabase{TAsset}"/>, after which this value adding in <see cref="IAddressablesDatabase.LoadPriorities"/>.
    /// The smaller the higher priority.
    /// Zero is default priority
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class LoadPriorityAttribute : Attribute
    {
        public int Priority { get; }

        public LoadPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
