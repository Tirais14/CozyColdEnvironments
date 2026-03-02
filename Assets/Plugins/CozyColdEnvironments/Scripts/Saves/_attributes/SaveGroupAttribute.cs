using CCEnvs.Attributes;
using System;

#nullable enable
namespace CCEnvs.Saves
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class SaveGroupAttribute : Attribute, ICCAttribute
    {
        public string Name { get; }
        public string? ID { get; }

        public SaveGroupAttribute(string name, string? id = null)
        {
            Name = name;
            ID = id;
        }
    }
}
