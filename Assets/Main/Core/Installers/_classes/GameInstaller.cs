using Core.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

#nullable enable
namespace Core.Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindInputActionsAsset();
        }

        private void BindPlayerInputHandler(InputActionMap playerActionMap)
        {
            var handler = new PlayerInputHandler(playerActionMap);

            Container.BindInstance(handler).AsSingle();
        }

        private void BindPlayerActionMap(InputActionAsset inputActionAsset)
        {
            var actionMap = inputActionAsset.FindActionMap("Player", throwIfNotFound: true);

            Container.BindInstance(actionMap).AsSingle();

            BindPlayerInputHandler(actionMap);
        }

        private void BindInputActionsAsset()
        {
            var inputActionAsset = Resources.Load<InputActionAsset>("InputSystem_Actions");

            Container.BindInstance(inputActionAsset).AsSingle();

            BindPlayerActionMap(inputActionAsset);
        }
    }
}
