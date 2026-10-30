using CCEnvs.Attributes;
using CCEnvs.Disposables;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
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
    public abstract class ViewElement
    {
        private static readonly Lazy<MemoryCache> rootDependentElementsCached = new(
            () => new MemoryCache(new MemoryCacheOptions
            {
                ExpirationScanFrequency = 1.Minutes()
            }));

        private static readonly Dictionary<Type, bool> hasRootBindableElementsFlags = new();

        [OnInstallExecutable]
        private static void OnInstall()
        {
            if (rootDependentElementsCached.IsValueCreated)
                rootDependentElementsCached.Value.Clear();

            hasRootBindableElementsFlags.Clear();
        }

        public static IReadOnlyList<FieldInfo> ResolveRootDependentFields(Type viewModelType)
        {
            Guard.IsNotNull(viewModelType, nameof(viewModelType));

            if (hasRootBindableElementsFlags.TryGetValue(viewModelType, out bool hasBindableElements) &&
                !hasBindableElements
                )
                return Array.Empty<FieldInfo>();

            if (rootDependentElementsCached.TryGetValue(out var rootDependentElements) &&
                rootDependentElements.TryGetValue<List<FieldInfo>>(viewModelType, out var fieldsCache) &&
                fieldsCache is not null)
            {
                return fieldsCache;
            }

            FieldInfo[] fields = viewModelType.GetFields(BindingFlagsDefault.InstanceAll);
            var toCacheFields = LightLazy.Create(fields.Length, static fieldCount => new List<FieldInfo>(fieldCount));

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                if (field.IsInitOnly ||
                    !field.IsDefined<RootDependentAttribute>(inherit: true)
                    )
                    continue;

                toCacheFields.Value.Add(field);
            }

            if (toCacheFields.IsValueCreated && toCacheFields.Value.Count >= 1)
            {
                toCacheFields.Value.TrimExcess();
                using var cacheEntry = rootDependentElementsCached.Value.CreateEntry(viewModelType);
                cacheEntry.AbsoluteExpirationRelativeToNow = 10.Minutes();
                cacheEntry.Value = toCacheFields.Value;
                hasRootBindableElementsFlags[viewModelType] = true;
                return toCacheFields.Value;
            }

            hasRootBindableElementsFlags[viewModelType] = false;
            return Array.Empty<FieldInfo>();
        }
    }

    public abstract class ViewElement<TViewModel> : View<TViewModel>
        where TViewModel : IViewModel
    {
        private readonly Lazy<List<IDisposable>> rootElementDisposables = new(() => new());

        private IDisposable? rootElementBinding;

        protected ICollection<IDisposable> RootElementBindings => rootElementDisposables.Value;

        protected override void Start()
        {
            base.Start();
            BindRootElement();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CCDisposable.Dispose(ref rootElementBinding);
            OnRootElementChangedInternal(null);
        }

        protected virtual void OnRootElementChanged(VisualElement? root) { }

        protected virtual void InitRootElement(VisualElement root) { }

        protected virtual void CleanupRootElement() { }

        protected virtual void OnRootElementOrViewModelChanged(VisualElement? root, TViewModel? viewModel) { }

        protected virtual void InitRootElementAndViewModel(VisualElement root, TViewModel viewModel) { }

        protected virtual void CleanupRootElementAndViewModel() { }

        private void OnRootElementChangedInternal(VisualElement? root)
        {
            OnRootElementChanged(root);
            
            if (root is null)
            {
                CleanupRootElement();
                ResetRootBindableElements();
            }
            else
                InitRootElement(root);

            DisposeViewModelBindings();
            OnSetViewModel(ViewModel);

            if (ViewModel.IsNotNull())
                InitViewModel(ViewModel);
            else
                CleanupViewModel();

            OnRootElementOrViewModelChanged(root, ViewModel);

            if (root is not null && ViewModel.IsNotNull())
                InitRootElementAndViewModel(root, ViewModel);
        }

        private void BindRootElement()
        {
            rootElementBinding = ElementShowable.ObserveRootElement()
                .Subscribe(OnRootElementChangedInternal);
        }

        private void ResetRootBindableElements()
        {
            foreach (var field in ViewElement.ResolveRootDependentFields(GetType()))
                field.SetValue(this, null);
        }
    }
}
