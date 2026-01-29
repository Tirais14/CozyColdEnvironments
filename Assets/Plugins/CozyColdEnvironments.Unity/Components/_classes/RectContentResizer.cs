using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Components
{
    [ExecuteAlways]
    public class RectContentResizer : CCBehaviour
    {
        private readonly List<RectTransform> childs = new();

        [SerializeField]
        private Maybe<Transform> anchorOverride;

        [NonSerialized]
        new private RectTransform transform = null!;

        [Range(1, 100)]
        [SerializeField]
        private int updateEveryFrame = 3;

        private int framePassed;

        protected override void Awake()
        {
            base.Awake();
            CacheTransform();
        }

        protected override void Start()
        {
            base.Start();
            CollectChilds();
        }

        private void OnTransformChildrenChanged()
        {
            UniTask.Create(this,
                static async @this =>
                {
                    await UniTask.WaitForEndOfFrame();
                    @this.CollectChilds();
                })
                .AttachExternalCancellation(destroyCancellationToken)
                .Forget();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                CacheTransform();
#endif

            if (transform == null)
                return;

            framePassed++;

            if (framePassed < updateEveryFrame)
                return;

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                CollectChilds();
#endif

            FitMaxSize();
            framePassed = 0;
        }

        private void CollectChilds()
        {
            this.childs.Clear();

            var childs = GetComponentsInChildren<RectTransform>(includeInactive: true);

            int childCount = childs.Length;
            this.childs.Capacity = childs.Length;
            RectTransform child;

            for (int i = 0; i < childCount; i++)
            {
                child = childs[i];

                if (child == transform)
                    continue;

                this.childs.Add(child);
            }
        }

        private bool TryGetMaxSize(out Vector2 maxSize)
        {
            maxSize = Vector2.zero;
            int childCount = childs.Count;

            if (childCount < 1)
                return false;

            RectTransform child;

            for (int i = 0; i < childCount; i++)
            {
                child = childs[i];

                if (!child.gameObject.activeSelf)
                    continue;

                if (child.sizeDelta.sqrMagnitude > maxSize.sqrMagnitude)
                    maxSize = child.sizeDelta;
            }

            return true;
        }

        private void FitMaxSize()
        {
            if (!TryGetMaxSize(out var maxSize))
                return;

            transform.sizeDelta = maxSize;

            if (anchorOverride.TryGetValue(out var anchor))
                transform.position = anchor.position;
        }

        private void CacheTransform()
        {
            transform = (RectTransform)base.transform;
        }
    }
}
