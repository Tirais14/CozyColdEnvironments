using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs
{
    public static class ObservableHelper
    {
        public static Observable<TObservableValue> MergeMany<TItem, TObservableValue>(
            this IEnumerable<TItem> items,
            Func<TItem, Observable<TObservableValue>> observableFactory
            )
        {
            CC.Guard.IsNotNull(items, nameof(items));

            if (items.IsEmpty())
                return Observable.Empty<TObservableValue>();

            Guard.IsNotNull(observableFactory, nameof(observableFactory));

            Observable<TObservableValue>? observable = null;

            foreach (var item in items)
            {
                if (observable is null)
                {
                    observable = observableFactory(item);
                    continue;
                }

                observable = observable.Merge(observableFactory(item));
            }

            return observable!;
        }
    }
}
