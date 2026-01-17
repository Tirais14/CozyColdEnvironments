using CCEnvs.Unity.Items;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
#pragma warning disable S101
namespace CCEnvs.Unity
{
    public delegate UniTask<TOutput> ConverterAsync<in TInput, TOutput>(TInput input);

    public static class UCC
    {
        private readonly static WeakLazy<Canvas> _devCanvasPrefab = new(
            static () =>
            {
                return Resources.Load<GameObject>("Prefabs/___DevCanvas")
                    .Q()
                    .Component<Canvas>()
                    .Strict();
            });

        private readonly static WeakLazy<Canvas> _devCanvas = new(
            static () =>
            {
                var canvasPrefab = _devCanvasPrefab.Value;

                var canvas = Object.Instantiate(canvasPrefab);

                canvas.transform.position = new Vector3(-25000f, -25000f);

                return canvas;
            });

        private readonly static WeakLazy<GameObject> _pooledObjectsParent = new(
            static () =>
            {
                return new GameObject("___Pooled");
            });

        public static Lazy<Sprite> ColorSprite { get; } = new(static () => Resources.Load<Sprite>("Textures/ColorSprite"));
        public static Lazy<Sprite> Transparent { get; } = new(static () => Resources.Load<Sprite>("Textures/DummySprite"));
        public static Lazy<Sprite> RedCrossSprite { get; } = new(static () => Resources.Load<Sprite>("Textures/RedCross"));
        public static Lazy<IInventory> WorldInventory { get; } = new(static () => new Inventory());
        public static Lazy<Material> MockMaterial { get; } = new(static () => Resources.Load<Material>("CC/Mock_Material"));
        public static Canvas DevCanvas => _devCanvas.Value;
        public static GameObject PooledObjectsParent => _pooledObjectsParent.Value;
    }
}
