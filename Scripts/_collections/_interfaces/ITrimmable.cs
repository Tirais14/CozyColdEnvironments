#nullable enable
namespace CCEnvs
{
    /// <summary>
    /// For auto expandable collections for memory optimizations
    /// </summary>
    public interface ITrimmable
    {
        void TrimExcess();
    }
}
