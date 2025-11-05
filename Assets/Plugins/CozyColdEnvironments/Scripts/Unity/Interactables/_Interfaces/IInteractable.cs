#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using SuperLinq;
using System;
using System.Linq;

namespace CCEnvs.Unity.Interactables
{
    public interface IInteractable
    {
        /// <returns>Left is message, right is returned value</returns>
        Either<InvokeInfo, Maybe<object>[]> Interact(params object?[]? tools);
    }
    public interface IInteractable<in TIn>
        : IInteractable
    {
        /// <inheritdoc cref="IInteractable.Interact(object?)"/>
        Either<InvokeInfo, Maybe<object>[]> Interact(params TIn?[]? tools);

        Either<InvokeInfo, Maybe<object>[]> IInteractable.Interact(params object?[]? tools)
        {
            return Interact(tools.Cast<TIn>().ToArray());
        }
    }
    public interface IInteractable<in TIn, TOut> 
        : IInteractable<TIn>
    {
        /// <inheritdoc cref="IInteractable.Interact(object?)"/>
        new Either<InvokeInfo, Maybe<TOut>[]> Interact(params TIn?[]? tools);

        Either<InvokeInfo, Maybe<object>[]> IInteractable<TIn>.Interact(params TIn?[]? tools)
        {
            var t = Interact(tools);

            return (t.LeftTarget, t.IfRight(
                arr => arr.Select(
                    arr => arr.Raw.As<object>().Maybe()).ToArray())
                .AccessRight(Array.Empty<Maybe<object>>())
                );
        }
    }
}
