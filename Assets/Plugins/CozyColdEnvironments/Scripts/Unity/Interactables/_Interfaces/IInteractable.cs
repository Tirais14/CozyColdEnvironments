#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using System;
using System.Linq;

namespace CCEnvs.Unity.Interactables
{
    public interface IInteractable
    {
        /// <returns>Left is message, right is returned value</returns>
        Either<string, object[]> Interact(params object?[]? tools);
    }
    public interface IInteractable<in TIn>
        : IInteractable
    {
        /// <inheritdoc cref="IInteractable.Interact(object?)"/>
        Either<string, object[]> Interact(params TIn?[]? tools);

        Either<string, object[]> IInteractable.Interact(params object?[] tools)
        {
            return Interact(tools.Cast<TIn>().ToArray());
        }
    }
    public interface IInteractable<in TIn, TOut> 
        : IInteractable<TIn>
    {
        /// <inheritdoc cref="IInteractable.Interact(object?)"/>
        new Either<string, TOut[]> Interact(params TIn?[]? tools);

        Either<string, object[]> IInteractable<TIn>.Interact(params TIn?[]? tools)
        {
            var t = Interact(tools);

            return (t.LeftTarget, t.IfRight(x => x.Cast<object>().ToArray()).RightTarget);
        }
    }
}
