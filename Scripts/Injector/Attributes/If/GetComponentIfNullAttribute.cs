using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.Injector
{
    [SuppressMessage("", "S3993")]
    public class GetComponentIfNullAttribute : GetComponentAttribute
    {
        public GetComponentIfNullAttribute(GetComponentSettings settings = GetComponentSettings.GetComponent)
            : base(settings.ToIfNull())
        {
        }
    }
}