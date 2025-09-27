using CCEnvs.Diagnostics;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.Disposables
{
    public class DisposableBinding : IDisposable
    {
        private readonly Component? component;
        private readonly List<IDisposable> disposables;
        private readonly bool nullIsTrigger;

        public bool ReadyToDispose => nullIsTrigger && component == null;

        public DisposableBinding(Component? component, params IDisposable[] disposables)
        {
            this.component = component;
            this.disposables = new List<IDisposable>(disposables);

            nullIsTrigger = component != null;
        }
        public DisposableBinding(params IDisposable[] disposables)
            :
            this(component: null, disposables)
        {
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void Add(IDisposable disposable)
        {
            if (disposable.IsNull())
                throw new System.ArgumentNullException(nameof(disposable));

            disposables.Add(disposable);

            if (disposables.Capacity > disposables.Count)
                disposables.TrimExcess();
        }

        public void Dispose()
        {
            for (int i = 0; i < disposables.Count; i++)
                disposables[i].Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
