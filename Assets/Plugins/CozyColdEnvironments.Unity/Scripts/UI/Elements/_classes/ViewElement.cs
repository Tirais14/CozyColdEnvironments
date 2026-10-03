using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using R3;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public abstract class ViewElement<TViewModel> : View<TViewModel>
        where TViewModel : IViewModel
    {
        private static readonly Lazy<MemoryCache> rootBindableElementsCache = new(
            () => new MemoryCache(new MemoryCacheOptions
            {
                ExpirationScanFrequency = 1.Minutes()
            }));

        private static readonly Dictionary<Type, bool> hasRootBindableElementsFlags = new();

        private IDisposable? rootElementBinding;

        protected override void Start()
        {
            base.Start();
            BindRootElement();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CCDisposable.Dispose(ref rootElementBinding);
            OnRootElementChanged(null);
        }

        protected virtual void InitRootElement(VisualElement root)
        {

        }

        protected virtual void OnRootElementReset()
        {
        }

        protected virtual void OnRootElementChanged(VisualElement? root)
        {

        }

        private void OnRootElementChangedInternal(VisualElement? root)
        {
            OnRootElementChanged(root);

            if (root is null)
            {
                OnRootElementReset();
                ResetRootBindableElements();
            }
            else
                InitRootElement(root);
        }

        private void BindRootElement()
        {
            rootElementBinding = ElementShowable.ObserveRootElement()
                .Subscribe(OnRootElementChangedInternal);
        }

        private void ResetRootBindableElements()
        {
            var type = GetType();

            if (hasRootBindableElementsFlags.TryGetValue(type, out bool hasBindableElements) &&
                !hasBindableElements
                )
                return;

            if (rootBindableElementsCache.TryGetValue(out var rootBindableElements) &&
                rootBindableElements.TryGetValue<List<FieldInfo>>(type, out var fieldsCache) &&
                fieldsCache is not null)
            {
                foreach (var field in fieldsCache)
                    field.SetValue(this, null);

                return;
            }

            FieldInfo[] fields = type.GetFields(BindingFlagsDefault.InstanceAll);
            var toCacheFields = LightLazy.Create(fields.Length, static fieldCount => new List<FieldInfo>(fieldCount));

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                if (field.IsInitOnly ||
                    !field.IsDefined<RootBindableAttribute>(inherit: true) ||
                    field.GetValue(this).IsNot<VisualElement>()
                    )
                    continue;

                field.SetValue(this, null);
                toCacheFields.Value.Add(field);
            }

            if (toCacheFields.IsValueCreated)
            {
                toCacheFields.Value.TrimExcess();
                var cacheEntry = rootBindableElementsCache.Value.CreateEntry(type);
                cacheEntry.Value = toCacheFields.Value;
                cacheEntry.AbsoluteExpirationRelativeToNow = 10.Minutes();
                hasRootBindableElementsFlags.Add(type, true);
            }
            else
                hasRootBindableElementsFlags.Add(type, false);
        }
    }
}
