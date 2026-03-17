using CCEnvs;
using CCEnvs.Saves;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

#nullable enable
namespace Core.Installers
{
    public class GameInstaller : MonoInstaller, IInitializable
    {
        public override void InstallBindings()
        {
            CC.Install(Range.From("Tests*", "Core*"));

            BindSelf();
            BindInputActionsAsset();
        }

        private void BindSelf()
        {
            Container.BindInterfacesTo<GameInstaller>()
                .FromInstance(this)
                .AsSingle()
                .NonLazy();
        }

        private void BindPlayerInputHandler(InputActionMap playerActionMap)
        {
            var handler = new PlayerInputHandler(playerActionMap);

            Container.BindInstance(handler).AsSingle().NonLazy();
        }

        private void BindPlayerActionMap(InputActionAsset inputActionAsset)
        {
            var actionMap = inputActionAsset.FindActionMap("Player", throwIfNotFound: true);

            Container.BindInstance(actionMap).AsSingle().NonLazy();

            BindPlayerInputHandler(actionMap);
        }

        private void BindInputActionsAsset()
        {
            var inputActionAsset = Resources.Load<InputActionAsset>("InputSystem_Actions");

            Container.BindInstance(inputActionAsset).AsSingle().NonLazy();

            BindPlayerActionMap(inputActionAsset);
        }

        void IInitializable.Initialize()
        {
            InstallSaveSystem();

            SceneManager.LoadSceneAsync(1);
        }

        private void InstallSaveSystem()
        {
            var archives = SaveSystem.GetOrCreateArchives(
                Saves.GetArchivePath(Saves.SETTINGS_ARCHIVE_NAME),
                Saves.GetArchivePath(Saves.SAVE_ARCHIVE_NAME)
                );

            archives[0].GetOrCreateCatalog(
                Saves.SETTINGS_CATALOG_PATH
                )
                .GetOrCreateIncrementalGroup(
                Saves.DefaultCreateGroupParams.WithGroupName(Saves.SETTINGS_GROUP_NAME)
                );

            archives[1].GetOrCreateCatalog(
                Saves.PLAYER_CATALOG_PATH
                )
                .GetOrCreateIncrementalGroup(
                Saves.DefaultCreateGroupParams.WithGroupName(Saves.PLAYER_GROUP_NAME)
                );
        }
    }
}
