using System;

#nullable enable
namespace CCEnvs.UnityX.ComponentInjections
{
    public abstract class GetComponentAttribute : Attribute
    {
        public string? NameFilter { get; init; }
        public string? TagFilter { get; init; }
        public StringMatchSettings NameMatchSettings { get; set; } = StringMatchSettings.Default;
        public Type? RequireComponent { get; init; }
        public Type? DepthLimiter { get; init; }

        public bool IsOptional { get; init; }
    }
}
