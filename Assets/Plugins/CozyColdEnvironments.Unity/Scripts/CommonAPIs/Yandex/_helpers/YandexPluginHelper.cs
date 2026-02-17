using CCEnvs.Attributes;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Humanizer;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace CCEnvs.Unity.CommonAPIs.Yandex
{
    public static class YandexPluginHelper
    {
        private readonly static WeakLazy<GameObject> devGo = new(
            static () =>
            {
                var go = new GameObject("YandexDevGameObject");

                UnityEngine.Object.DontDestroyOnLoad(go);

                return go;
            });

        [OnInstallResetable]
        private static readonly Dictionary<string, ImageLoadYG> imageLoaders = new();

        public static async UniTask<Sprite?> LoadImageAsync(string imageUrl, CancellationToken cancellationToken = default)
        {
            if (imageUrl.IsNullOrWhiteSpace())
                return null;

            if (!imageLoaders.TryGetValue(imageUrl, out var loader))
            {
                var loaderGO = new GameObject($"YandexImageLoader:{imageUrl}");

                loaderGO.transform.SetParent(devGo.Value.transform);

                loader = loaderGO.AddComponent<ImageLoadYG>();

                var loaderImg = loaderGO.AddComponent<Image>();

                loader.spriteImage = loaderImg;

                loader.urlImage = imageUrl;

                imageLoaders.Add(imageUrl, loader);
            }

            if (loader.spriteImage.sprite == null)
            {
                loader.Load();

                await UniTask.WaitUntil(
                    loader,
                    static loader =>
                    {
                        return loader.spriteImage.sprite != null;
                    },
                    cancellationToken: cancellationToken
                    )
                    .TimeoutWithoutException(1.Minutes());
            }

            return loader.spriteImage.sprite;
        }
    }
}
