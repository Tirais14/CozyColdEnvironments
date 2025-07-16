#nullable enable

namespace UTIRLib.DB
{
    public interface IDatabaseFileInfo
    {
        string Path { get; }
        string Name { get; }
        bool IsLoadOnInitialize { get; }
    }
}