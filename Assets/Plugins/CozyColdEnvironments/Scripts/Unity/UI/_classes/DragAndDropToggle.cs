using CCEnvs.Diagnostics;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class DragAndDropToggle : IDragAndDropToggle
    {
        private readonly List<IDisposable> disposables = new(4);

        private readonly GameObject target;
        private readonly DragAndDropAction? onBeginDrag;
        private readonly DragAndDropAction? onDrag;
        private readonly DragAndDropAction? onEndDrag;
        private readonly DragAndDropAction? onDrop;

        public int BindingCount { get; private set; }

        public DragAndDropToggle(
            GameObject target,
            DragAndDropAction? onBeginDrag = null!,
            DragAndDropAction? onDrag = null!,
            DragAndDropAction? onEndDrag = null!,
            DragAndDropAction? onDrop = null!)
        {
            this.target = target;
            this.onBeginDrag = onBeginDrag;
            this.onDrag = onDrag;
            this.onEndDrag = onEndDrag;
            this.onDrop = onDrop;
        }

        public static implicit operator bool(DragAndDropToggle source)
        {
            return source.BindingCount > 0;
        }

        public void ActivateDragAndDropAbility()
        {
            if (BindingCount > 0)
            {
                BindingCount++;
                return;
            }

            if (target.TryGetComponent<DragHandler>(out var dragHandler))
            {
                if (onBeginDrag is not null)
                {
                    disposables.Add(dragHandler.ObserveOnBeginDrag()
                                               .Subscribe(x => onBeginDrag(x))
                                               .AddTo(target));
                }

                if (onDrag is not null)
                {
                    disposables.Add(dragHandler.ObserveOnDrag()
                                               .Subscribe(x => onDrag(x))
                                               .AddTo(target));
                }

                if (onEndDrag is not null)
                {
                    disposables.Add(dragHandler.ObserveOnEndDrag()
                                               .Subscribe(x => onEndDrag(x))
                                               .AddTo(target));
                }
            }
            else
                this.PrintLog($"Cannot find {nameof(DragHandler)}. Drag is disabled.");

            if (target.TryGetComponent<DropHandler>(out var dropHandler))
            {
                if (onDrop is not null)
                {
                    disposables.Add(dropHandler.ObserveOnDrop()
                                               .Subscribe(x => onDrop(x))
                                               .AddTo(target));
                }
            }
            else
                this.PrintLog($"Cannot find {nameof(DropHandler)}. Drop is disabled.");

            BindingCount++;
        }

        public void DeactivateDragAndDropAbility()
        {
            if (BindingCount <= 0)
                return;

            disposables!.ForEach(x => x.Dispose());
            disposables.Clear();

            BindingCount--;
        }
    }
}
