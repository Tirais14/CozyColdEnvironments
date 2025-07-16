namespace UTIRLib.DB
{
    public interface IDatabaseBase
    {
        int Count { get; }
        bool IsLoaded { get; }
    }
}