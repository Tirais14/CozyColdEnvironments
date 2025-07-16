using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.Injector
{
    [SuppressMessage("", "S3993")]
    public class GetComponentInParentAttribute : GetComponentAttribute
    {
        public string? ParentName { get; set; }
        public bool HasParentName => ParentName is not null;

        public GetComponentInParentAttribute(string? parentName,
            GetComponentSettings settings = GetComponentSettings.GetComponent) : base(settings) =>
            ParentName = parentName;

        public GetComponentInParentAttribute(GetComponentSettings settings = GetComponentSettings.GetComponent)
            : this(null, settings)
        {
        }
    }
}