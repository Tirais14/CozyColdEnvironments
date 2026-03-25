using System;

#nullable enable
namespace CCEnvs.Unity.Injections
{
    public abstract class GetComponentAttribute : Attribute
    {
        public string? NameFilter { get; init; }
        public string? TagFilter { get; init; }
        public StringMatchSettings? NameMatchSettings { get; set; }

        public bool IsOptional { get; init; }
    }
}
