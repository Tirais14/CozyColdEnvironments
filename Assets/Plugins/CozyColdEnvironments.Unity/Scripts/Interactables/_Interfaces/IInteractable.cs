#nullable enable
using CCEnvs.FuncLanguage;

namespace CCEnvs.Unity.Interactables
{
    public interface IInteractable
    {
        bool RequireInput { get; }
        bool HasOutput { get; }

        bool TryInteract(object? input, out Maybe<object> result);
    }
    public interface IInteractable<in TIn>
        : IInteractable
    {
        bool TryInteract(TIn? input, out Maybe<object> result);

        bool IInteractable.TryInteract(object? input, out Maybe<object> result)
        {
            return TryInteract(input.AsObsolete<TIn>().Raw, out result);
        }
    }
    public interface IInteractable<in TIn, TOut>
        : IInteractable<TIn>
    {
        bool TryInteract(TIn? input, out Maybe<TOut> result);

        bool IInteractable<TIn>.TryInteract(TIn? input, out Maybe<object> result)
        {
            var t = TryInteract(input, out Maybe<TOut> resultTyped);
            result = resultTyped;

            return t;
        }
    }
}
