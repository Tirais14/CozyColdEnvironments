using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.Injector
{
    [SuppressMessage("", "S3993")]
    public class GetComponentInChildrenAttribute : GetComponentAttribute
    {
        public string? ChildName { get; set; }
        public bool HasChildName => ChildName is not null;

        public GetComponentInChildrenAttribute(string? childName,
            GetComponentSettings settings = GetComponentSettings.GetComponent) : base(settings) => ChildName = childName;

        public GetComponentInChildrenAttribute(GetComponentSettings settings = GetComponentSettings.GetComponent)
            : this(null, settings)
        {
        }
    }
}