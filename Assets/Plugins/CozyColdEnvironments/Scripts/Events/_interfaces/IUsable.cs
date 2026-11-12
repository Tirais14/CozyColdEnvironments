#nullable enable
namespace CCEnvs.Events
{
    public interface IUsable
    {
        bool IsUsed { get; set; }
    }

    public static class IUsableExtensions
    {
        public static void Use(this IUsable usable)
        {
            CC.Guard.IsNotNull(usable, nameof(usable));
            usable.IsUsed = true;
        }
    }
}
