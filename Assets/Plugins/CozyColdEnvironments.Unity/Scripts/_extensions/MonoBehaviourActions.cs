namespace CCEnvs.Unity
{
    public static class MonoBehaviourActions
    {
        //public static void DoAction<TMonoBehaviour>(this TMonoBehaviour source,
        //    Func<TMonoBehaviour, UniTask> awaiter, Action<TMonoBehaviour> action)
        //    where TMonoBehaviour : MonoBehaviour
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(awaiter);
        //    Guard.IsNotNull(action);

        //    UniTask.Create((source, awaiter, action),
        //        static async input =>
        //        {
        //            await input.awaiter(input.source);
        //            input.action(input.source);
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoAction<TMonoBehaviour>(this TMonoBehaviour source,
        //    Func<UniTask> awaiter, Action<TMonoBehaviour> action)
        //    where TMonoBehaviour : MonoBehaviour
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(awaiter);
        //    Guard.IsNotNull(action);

        //    UniTask.Create((source, awaiter, action),
        //        static async input =>
        //        {
        //            await input.awaiter();
        //            input.action(input.source);
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoAction<TMonoBehaviour>(this TMonoBehaviour source,
        //    Func<TMonoBehaviour, UniTask> awaiter, Action action)
        //    where TMonoBehaviour : MonoBehaviour
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(awaiter);
        //    Guard.IsNotNull(action);

        //    UniTask.Create((source, awaiter, action),
        //        static async input =>
        //        {
        //            await input.awaiter(input.source);
        //            input.action();
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoAction<TMonoBehaviour, T>(this TMonoBehaviour source,
        //    T state,
        //    Func<TMonoBehaviour, T, UniTask> awaiter, Action<TMonoBehaviour, T> action)
        //    where TMonoBehaviour : MonoBehaviour
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(awaiter);
        //    Guard.IsNotNull(action);

        //    UniTask.Create((source, awaiter, action, state),
        //        static async input =>
        //        {
        //            await input.awaiter(input.source, input.state);
        //            input.action(input.source, input.state);
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoAction<TMonoBehaviour, T>(this TMonoBehaviour source,
        //    T state,
        //    Func<TMonoBehaviour, UniTask> awaiter, Action<TMonoBehaviour, T> action)
        //    where TMonoBehaviour : MonoBehaviour
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(awaiter);
        //    Guard.IsNotNull(action);

        //    UniTask.Create((source, awaiter, action, state),
        //        static async input =>
        //        {
        //            await input.awaiter(input.source);
        //            input.action(input.source, input.state);
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoAction<TMonoBehaviour, T>(this TMonoBehaviour source,
        //    T state,
        //    Func<UniTask> awaiter, Action<TMonoBehaviour, T> action)
        //    where TMonoBehaviour : MonoBehaviour
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(awaiter);
        //    Guard.IsNotNull(action);

        //    UniTask.Create((source, awaiter, action, state),
        //        static async input =>
        //        {
        //            await input.awaiter();
        //            input.action(input.source, input.state);
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoAction<TMonoBehaviour, T>(this TMonoBehaviour source,
        //    T state,
        //    Func<TMonoBehaviour, T, UniTask> awaiter, Action<TMonoBehaviour> action)
        //    where TMonoBehaviour : MonoBehaviour
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(awaiter);
        //    Guard.IsNotNull(action);

        //    UniTask.Create((source, awaiter, action, state),
        //        static async input =>
        //        {
        //            await input.awaiter(input.source, input.state);
        //            input.action(input.source);
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoAction<TMonoBehaviour, T>(this TMonoBehaviour source,
        //    T state,
        //    Func<TMonoBehaviour, T, UniTask> awaiter, Action action)
        //    where TMonoBehaviour : MonoBehaviour
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(awaiter);
        //    Guard.IsNotNull(action);

        //    UniTask.Create((source, awaiter, action, state),
        //        static async input =>
        //        {
        //            await input.awaiter(input.source, input.state);
        //            input.action();
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoAction(this MonoBehaviour source, Func<UniTask> awaiter, Action action)
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(awaiter);
        //    Guard.IsNotNull(action);

        //    UniTask.Create((source, awaiter, action),
        //        static async input =>
        //        {
        //            await input.awaiter();
        //            input.action();
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoAction<T>(this MonoBehaviour source, T state, Func<T, UniTask> awaiter, Action<T> action)
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(awaiter);
        //    Guard.IsNotNull(action);

        //    UniTask.Create((source, awaiter, action, state),
        //        static async input =>
        //        {
        //            await input.awaiter(input.state);
        //            input.action(input.state);
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoAction<T>(this MonoBehaviour source, T state, Func<UniTask> awaiter, Action<T> action)
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(awaiter);
        //    Guard.IsNotNull(action);

        //    UniTask.Create((source, awaiter, action, state),
        //        static async input =>
        //        {
        //            await input.awaiter();
        //            input.action(input.state);
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoActionAsync(this MonoBehaviour source, Func<UniTask> action)
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(action);

        //    UniTask.Create(action,
        //        static async action =>
        //        {
        //            await action();
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoActionAsync<T>(this MonoBehaviour source, T state, Func<T, UniTask> action)
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(action);

        //    UniTask.Create((action, state),
        //        static async input =>
        //        {
        //            await input.action(input.state);
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoActionAsync<TMonoBehaviour>(this TMonoBehaviour source, Func<TMonoBehaviour, UniTask> action)
        //    where TMonoBehaviour : MonoBehaviour
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(action);

        //    UniTask.Create((action, source),
        //        static async input =>
        //        {
        //            await input.action(input.source);
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}

        //public static void DoActionAsync<TMonoBehaviour, T>(this TMonoBehaviour source, T state, Func<TMonoBehaviour, T, UniTask> action)
        //    where TMonoBehaviour : MonoBehaviour
        //{
        //    CC.Guard.IsNotNull(source, nameof(source));
        //    Guard.IsNotNull(action);

        //    UniTask.Create((action, source, state),
        //        static async input =>
        //        {
        //            await input.action(input.source, input.state);
        //        })
        //        .AttachExternalCancellation(source.destroyCancellationToken)
        //        .SuppressCancellationThrow()
        //        .Forget();
        //}
    }
}
