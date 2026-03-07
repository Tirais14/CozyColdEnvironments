using System;
using CCEnvs.Diagnostics;
using R3;
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

            var args =
            (
                Instance: source,
                SiblingIdx: source.GetSiblingIndex(),
                Parent: source.parent,
                LocalPosition: source.localPosition
            );

            source.SetParent(UCC.DevCanvas.transform);

            return Disposable.Create(args,
                static args =>
                {
                    if (args.Instance == null
                        ||
                        args.Parent == null)
                    {
                        CCDebug.Instance.PrintLog($"The {nameof(args.Instance)} was destroyed and operation of return to a normal canvas is canceled");
                        return;
                    }

                    args.Instance.SetParent(args.Parent);
                    args.Instance.SetSiblingIndex(args.SiblingIdx);
                    args.Instance.localPosition = args.LocalPosition;
                });
        }
    }
}
