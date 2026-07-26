using CCEnvs.FuncLanguage;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public sealed class DragEvent : IPoolable
    {
        private Action<IPoolable>? onDespawnCallback;

        public VisualElement Source { get; private set; } = null!;
        public VisualElement? Target { get; private set; } = null!;

        public GameObject SourceGameObject { get; private set; } = null!;
        public GameObject? TargetGameObject { get; private set; } = null!;

        public IPointerEvent Info { get; private set; } = null!;

        event Action<IPoolable> IPoolable.OnDespawnCallback {
            add => onDespawnCallback += value;
            remove => onDespawnCallback -= value;
        }

        Maybe<PooledObject> IPoolable.PoolHandle { get; set; }

        bool IPoolable.IsValid => true;

        public DragEvent(
            VisualElement source,
            GameObject sourceGameObject,
            IPointerEvent ev
            )
        {
            Guard.IsNotNull(source);
            CC.Guard.IsNotNull(sourceGameObject, nameof(sourceGameObject));

            Source = source;
            Target = source;
            SourceGameObject = sourceGameObject;
            TargetGameObject = sourceGameObject;
            Info = ev;
        }

        public DragEvent() { }

        public DragEvent SetSource(VisualElement source, GameObject sourceGameObject)
        {
            Guard.IsNotNull(source);
            CC.Guard.IsNotNull(sourceGameObject, nameof(sourceGameObject));
            Source = source;
            SourceGameObject = sourceGameObject;
            return this;
        }

        public DragEvent SetTarget(VisualElement? target, GameObject? targetGameObject)
        {
            Target = target.IfNull(Source);
            TargetGameObject = targetGameObject.IfNull(targetGameObject);
            return this;
        }

        public DragEvent SetInfo(IPointerEvent info)
        {
            Info = info;
            return this;
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Source), Source)
                .AddProperty(nameof(Target), Target)
                .AddProperty(nameof(SourceGameObject), SourceGameObject)
                .AddProperty(nameof(TargetGameObject), TargetGameObject)
                .AddProperty(nameof(Info), Info)
                .ToStringAndDispose();
        }

        void IPoolable.OnDespawned()
        {
            Source = null!;
            SourceGameObject = null!;
            Target = null!;
            TargetGameObject = null!;
            Info = null!;
        }

        void IPoolable.OnSpawned()
        {
        }

        bool IPoolable.ReturnToPool()
        {
            return ((IPoolable)this).PoolHandle.IfSome(x => x.Dispose()).IsSome;
        }

        void IUtilizable.Utilize() => ((IPoolable)this).ReturnToPool();
    }
}
