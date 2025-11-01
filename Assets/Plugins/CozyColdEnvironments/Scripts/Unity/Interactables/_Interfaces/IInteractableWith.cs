#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;

namespace CCEnvs.Unity.Interactables
{
    public interface IInteractableWith : IInteractableBase
    {
        new Maybe<object> Interact(object tool);

        Either<string, object> IInteractableBase.Interact(object? tool)
        {
            if (tool.IsNull())
                return ($"tool required for interaction.", null);

            Interact(tool);
            return Either<string, object>.None;
        }
    }
    public interface IInteractableWith<in TIn> : IInteractableWith
    {
        Maybe<object> Interact(TIn tool);

        Maybe<object> IInteractableWith.Interact(object tool) => Interact(tool);
        Either<string, object> IInteractableBase.Interact(object? tool)
        {
            if (tool.IsNull())
                return ($"tool (type: {typeof(TIn).GetFullName()}) required for interaction.", null);

            return (null, Interact(tool));
        }
    }
    public interface IInteractableWith<in TIn, TOut> : IInteractableWith<TIn>
    {
        new Maybe<TOut> Interact(TIn tool);

        Maybe<object> IInteractableWith<TIn>.Interact(TIn tool) => Interact(tool);
        Maybe<object> IInteractableWith.Interact(object tool) => Interact(tool);
        Either<string, object> IInteractableBase.Interact(object? tool)
        {
            if (tool.IsNull())
                return ($"tool (type: {typeof(TIn).GetFullName()}) required for interaction.", null);

            return (null, Interact(tool));
        }
    }
}
