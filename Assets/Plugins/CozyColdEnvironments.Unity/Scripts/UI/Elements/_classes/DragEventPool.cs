using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public class DragEventPool : ObjectPool<DragEvent>
    {
        public static DragEventPool Shared { get; } = new();

        public DragEventPool(
            IFactory<DragEvent>? factory = null,
            int capacity = 4,
            int? maxSize = null)
            :
            base(factory, capacity, maxSize)
        {
        }

        [Obsolete("Is not supported", true)]
        new public PooledObject<DragEvent> Get() => throw new System.NotSupportedException("Get without arguments is not allowed");

        public PooledObject<DragEvent> Get(
            VisualElement source,
            GameObject sourceGameObject,
            IPointerEvent ev
            )
        {
            PooledObject<DragEvent> handle = base.Get();
            handle.Value.Source = source;
            handle.Value.SourceGameObject = sourceGameObject;
            handle.Value.Event = ev;
            return handle;
        }
    }
}
