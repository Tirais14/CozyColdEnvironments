using CCEnvs.Attributes;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.Saves;
using CCEnvs.Unity.Snapshots;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

        /// <summary>
        /// If !(<see cref="Application.isPlaying"/>) returns -1
        /// </summary>
        public static long CurrentFrame {
            get
            {
                if (Application.isPlaying)
                    return Time.frameCount - 1;

                return -1;
            }
        }

        [OnInstallMethod]
        private static void SetupNullValidation()
        {
            NullValidation.SetOverride(static
                obj =>
                {
                    if (obj is Object uObj)
                    {
                        if (obj is CCBehaviour beh)
                            return beh.IsDestroyed;

                        if (Thread.CurrentThread.IsMainThread())
                            return uObj.Equals(null);
                    }

                    return false;
                });
        }

        #region Install
        [OnInstallMethod]
        private static void InstallSavingSystem()
        {
            SavingSystem.Self.RegisterType<GameObject>(
                static (go) =>
                {
                    return new GameObjectSnapshot(go);
                });

            SavingSystem.Self.RegisterType<Slider>(
                static (slider) =>
                {
                    return new SliderSnapshot(slider);
                });

            SavingSystem.Self.RegisterType<TMP_Dropdown>(
                static (dropdown) =>
                {
                    return new TMP_DropdownSnapshot(dropdown);
                });
        }
        #endregion Install
    }
}
