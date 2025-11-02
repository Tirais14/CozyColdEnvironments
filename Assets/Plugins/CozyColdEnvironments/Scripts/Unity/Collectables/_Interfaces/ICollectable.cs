using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Interactables;
using System;

#nullable enable
namespace CCEnvs.Unity
{
    [Obsolete("Maybe better use IInterctable")]
    public interface ICollectable
    {
        object? Collect();
    }
    [Obsolete("Maybe better use IInterctable")]
    public interface ICollectable<out T> : ICollectable
    {
        new T Collect();

        object? ICollectable.Collect() => Collect();
    }
}
