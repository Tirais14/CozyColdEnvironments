using System;

#nullable enable
namespace CCEnvs.UnityX.Injections
{
    public abstract class GetComponentAttribute : Attribute
    {
        public string? NameFilter { get; init; }
        public string? TagFilter { get; init; }
        public StringMatchSettings NameMatchSettings { get; set; } = StringMatchSettings.Default;

        public bool IsOptional { get; init; }
    }
}
