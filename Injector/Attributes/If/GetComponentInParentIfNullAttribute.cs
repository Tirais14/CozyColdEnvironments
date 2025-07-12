using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.Injector
{
    [SuppressMessage("", "S3993")]
    public class GetComponentInParentIfNullAttribute : GetComponentInParentAttribute
    {
        public GetComponentInParentIfNullAttribute(string? parentName,
            GetComponentSettings settings = GetComponentSettings.GetComponent)
            : base(parentName, settings.ToIfNull())
        {
        }

        public GetComponentInParentIfNullAttribute(GetComponentSettings settings = GetComponentSettings.GetComponent)
            : this(null, settings)
        {
        }
    }
}