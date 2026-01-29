using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class CanvasHelper
    {
        public static IDisposable MoveToDevCanvas(this RectTransform source)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.parent.name.Contains("___DevCanvas"))
                return Disposable.Empty;

            var args = new
            {
                Instance = source,
                SiblingIdx = source.GetSiblingIndex(),
                Parent = source.parent,
                LocalPosition = source.localPosition
            };

            source.SetParent(UCC.DevCanvas.transform);

            return Disposable.Create(args,
                static args =>
                {
                    args.Instance.transform.SetParent(args.Parent);
                    args.Instance.transform.SetSiblingIndex(args.SiblingIdx);
                    args.Instance.transform.localPosition = args.LocalPosition;
                });
        }
    }
}
