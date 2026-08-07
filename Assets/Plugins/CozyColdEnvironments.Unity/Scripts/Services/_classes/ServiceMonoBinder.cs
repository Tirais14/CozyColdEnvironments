using CCEnvs.Disposables;
using CCEnvs.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Services
{
    [DisallowMultipleComponent]
    public sealed class ServiceMonoBinder : MonoBehaviour
    {
        public ServiceMonoBinderItem[] Infos = Array.Empty<ServiceMonoBinderItem>();

        public bool BindGameObject;

        private DisposableLight<CCServices.BindHandle>[]? bindings;

        private void Awake()
        {
            if (Infos is null)
                return;

            var bindingList = new List<DisposableLight<CCServices.BindHandle>>(BindGameObject ? Infos.Length + 1 : Infos.Length);

            for (int i = 0; i < Infos.Length; i++)
            {
                ServiceMonoBinderItem info = Infos[i];

                var binder = CCServices.BindInstance(info.Component);

                if (info.ID.IsNotNullOrWhiteSpace())
                {
                    if (int.TryParse(info.ID, out int intID))
                        binder.WithID(intID);
                    else if (long.TryParse(info.ID, out long longID))
                        binder.WithID(longID);
                    else
                        binder.WithID(info.ID);
                }

                if (info.Options.HasFlagT(ServiceMonoBinderOptions.WithBaseTypes))
                    binder.WithBaseTypes();

                if (info.Options.HasFlagT(ServiceMonoBinderOptions.WithInterfaces))
                {
                    if (info.InterfacesFilter.IsNotNullOrWhiteSpace())
                        binder.WithInterfaces(info.InterfacesFilter);
                    else
                        binder.WithInterfaces();
                }

                bindingList.Add(binder.AsSingle());
            }

            bindings = bindingList.ToArray();
        }

        private void OnDestroy()
        {
            if (bindings is null)
                return;

            for (int i = 0; i < bindings.Length; i++)
                bindings[i].Dispose();
        }
    }
}
