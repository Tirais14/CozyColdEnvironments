using CCEnvs.Attributes;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Components.Specialized;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.Saves;
using CCEnvs.Unity.Snapshots;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
                return new GameObject($"[{nameof(CCEnvs)} - PoolObjects]");
            });

        private readonly static Lazy<Sprite> anonymousProfileImage = new(
            static () =>
            {
                const string PATH = "CC/Profiles/Textures/AnonymousProfileImage";

                return Resources.Load<Sprite>(PATH)
                    .Maybe()
                    .GetValueUnsafe(static () => throw new InvalidOperationException($"Cannot load by path: {PATH}"));
            });

        private readonly static Lazy<Transform> devOjbect = new(
            static () =>
            {
                var go = new GameObject($"[{nameof(CCEnvs)} - Other]").transform;

                Object.DontDestroyOnLoad(go);

                return go;
            });

        public static Lazy<Sprite> ColorSprite { get; } = new(static () => Resources.Load<Sprite>("Textures/ColorSprite"));
       
        public static Lazy<Sprite> Transparent { get; } = new(static () => Resources.Load<Sprite>("Textures/DummySprite"));
       
        public static Lazy<Sprite> RedCrossSprite { get; } = new(static () => Resources.Load<Sprite>("Textures/RedCross"));
        
        public static Lazy<IInventory> WorldInventory { get; } = new(static () => new Inventory());
        
        public static Lazy<Material> MockMaterial { get; } = new(static () => Resources.Load<Material>("CC/Mock_Material"));
        
        public static Canvas DevCanvas => _devCanvas.Value;
       
        public static GameObject PooledObjectsParent => _pooledObjectsParent.Value;

        public static Sprite AnonymousProfileImage => anonymousProfileImage.Value;

        public static Transform DevObject => devOjbect.Value;

        /// <summary>
        /// If !(<see cref="Application.isPlaying"/>) returns -1
        /// </summary>
        public static long CurrentFrame {
            get
            {
                if (!Application.isPlaying)
                    return -1;

                return Time.frameCount - 1;
            }
        }

        [Obsolete("", error: true)]
        public static string BuildResourcePath(string path, AssetType assetType)
        {
            string? assetTypeFolder = assetType switch
            {
                AssetType.Prefab => "Prefabs",
                AssetType.ScriptableObject => "Configs",
                AssetType.Texture2D => "Textures",
                AssetType.Sprite => "Textures",
                AssetType.SpriteAtlas => "Textures",
                _ => null
            };

            if (assetTypeFolder is not null)
                return $"{nameof(CC)}/{path}/{assetTypeFolder}";

            return $"{nameof(CC)}/{path}";
        }

        #region Install
        [OnInstallExecutable]
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

        [OnInstallExecutable]
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

            SavingSystem.Self.RegisterType<Toggle>(
                static toggle =>
                {
                    return new ToggleSnapshot(toggle);
                });

            SavingSystem.Self.RegisterType<Component>(
                static cmp =>
                {
                    return new ComponentSnapshot<Component>(cmp);
                });

            SavingSystem.Self.RegisterType<Behaviour>(
                static beh =>
                {
                    return new BehaviourSnapshot<Behaviour>();
                });

            SavingSystem.Self.RegisterType<MonoBehaviour>(
                static beh =>
                {
                    return new MonoBehaviourSnapshot<MonoBehaviour>();
                });

            SavingSystem.Self.RegisterType<UIBehaviour>(
                static beh =>
                {
                    return new UIBehaviourSnapshot<UIBehaviour>();
                });

            SavingSystem.Self.RegisterType<Selectable>(
                static sel =>
                {
                    return new SelectableSnapshot<Selectable>();
                });
        }
        #endregion Install

        public static class Platform
        {
            public static bool IsWebGL {
                get
                {
#if PLATFORM_WEBGL
                    return true;
#elif UNITY_EDITOR
                    return PlatformDependentBehaviourEmulator.IsWebGL;
#else
                    return false;
#endif
                }
            }

            public static bool IsMobile {
                get
                {
                    return Application.isMobilePlatform || PlatformDependentBehaviourEmulator.IsMobile;
                }
            }

            public static bool IsConsole {
                get
                {
                    return Application.isConsolePlatform || PlatformDependentBehaviourEmulator.IsConsole;
                }
            }
        }
    }
}
