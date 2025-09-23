using System.Collections.Generic;
using System.Linq;
using CCEnvs.Reflection;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    public interface IEditorSerialized : ITransformable
    {
        object Output { get; }
        bool IsDefault => ObjectValidator.EqaulsDefaultByFields(this);

        object ITransformable.DoTransform() => Output;
    }
    public interface IEditorSerialized<out T> : IEditorSerialized, ITransformable<T>
    {
        new T Output { get; }

        object IEditorSerialized.Output => Output!;

        T ITransformable<T>.DoTransform() => Output;
        object ITransformable.DoTransform() => Output!;
    }

    public static class IEditorSerializedExtensions
    {
        public static IEnumerable<T> AsUnserialiazedValues<TWrapper, T>(this IEnumerable<TWrapper> wrappers)
            where TWrapper : IEditorSerialized<T>
        {
            return wrappers.Select(x => x.Output).ToArray();
        }
    }
}
