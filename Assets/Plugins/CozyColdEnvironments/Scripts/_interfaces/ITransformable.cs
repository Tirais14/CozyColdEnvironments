#nullable enable
using System;

namespace CCEnvs
{
    /// <summary>
    /// Converts to specified type
    /// </summary>
    public interface ITransformable
    {
        Type TransformationType { get; }

        object DoTransform();
    }
    public interface ITransformable<out T> : ITransformable
    {
        Type ITransformable.TransformationType => typeof(T);

        new T DoTransform();

        object ITransformable.DoTransform() => DoTransform()!;
    }
}
