#nullable enable
namespace CCEnvs.Reflection
{
    public abstract class ContextedMemberInfo 
    {
        public readonly object? context;

        protected ContextedMemberInfo(object? context)
        {
            this.context = context;
        }
    }
}
