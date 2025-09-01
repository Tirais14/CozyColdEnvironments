#nullable enable
namespace CCEnvs.Diagnostics
{
    public class ObjectDisabledException : CCException
    {
        public ObjectDisabledException()
        {
        }

        public ObjectDisabledException(object obj)
            :
            base($"{obj.GetTypeName()} is disabled.")
        {
        }
    }
}
