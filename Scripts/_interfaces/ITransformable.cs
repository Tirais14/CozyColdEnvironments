#nullable enable
using System;

namespace CCEnvs
{
    /// <summary>
    /// Converts to specified type
    /// </summary>
    public interface ITransformable
    {
        object DoTransform();
    }
    public interface ITransformable<out T> : ITransformable
    {
        new T DoTransform();

        object ITransformable.DoTransform() => DoTransform()!;
    }
}
