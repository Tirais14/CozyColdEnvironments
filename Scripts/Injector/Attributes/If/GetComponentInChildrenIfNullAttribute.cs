using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.Injector
{
    [SuppressMessage("", "S3993")]
    public class GetComponentInChildrenIfNullAttribute : GetComponentInChildrenAttribute
    {
        public GetComponentInChildrenIfNullAttribute(string? childName,
            GetComponentSettings settings = GetComponentSettings.GetComponent)
            : base(childName, settings.ToIfNull())
        {
        }

        public GetComponentInChildrenIfNullAttribute(GetComponentSettings settings = GetComponentSettings.GetComponent)
            : this(childName: null, settings)
        {
        }
    }
}