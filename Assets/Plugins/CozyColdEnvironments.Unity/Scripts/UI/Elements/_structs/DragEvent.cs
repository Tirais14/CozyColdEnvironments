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

        public VisualElement Source { get; internal set; } = null!;
        public VisualElement? Target { get; internal set; } = null!;

        public GameObject SourceGameObject { get; internal set; } = null!;
        public GameObject? TargetGameObject { get; internal set; } = null!;

        public IPointerEvent Info { get; internal set; } = null!;

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

        internal DragEvent() { }

        public DragEvent SetTarget(VisualElement? target, GameObject? targetGameObject)
        {
            Target = target;
            TargetGameObject = targetGameObject;
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
