using CCEnvs.Attributes;
using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Reflection
{
    public class MemberIDAttribute : Attribute, ICCAttribute
    {
        public string ID { get; }

        public MemberIDAttribute(string id)
        {
            Guard.IsNotNull(id, nameof(id));

            ID = id;
        }
    }
}
