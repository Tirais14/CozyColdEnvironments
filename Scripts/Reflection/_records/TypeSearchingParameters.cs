#nullable enable
namespace CozyColdEnvironments.Reflection
{
    public record TypeSearchingParameters
    {
        public string NamepsacePart { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public bool SearchByFullName { get; set; }
        public bool IgnoreCase { get; set; }

        public string FullTypeName => $"{NamepsacePart.TrimEnd('.')}.{TypeName}";
    }
}
