using CCEnvs.Collections;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Saves;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [RequireComponent(typeof(PersistentGuid))]
    public sealed class BindGameObjectToSaveSystem : CCBehaviour
    {
        [SerializeField]
        private SavingSystemToRegisterObject[] components = Array.Empty<SavingSystemToRegisterObject>();

        [SerializeField]
        [Tooltip("Process only specified in 'components' field components")]
        private bool onlyExplicitComponents = true;

        protected override void Awake()
        {
            base.Awake();

            gameObject.SavingSystemRegisterGameObject().AddTo(gameObject);

            if (components.IsNotNullOrEmpty())
            {   
                foreach (var cmp in components)
                    cmp.Object.SavingSystemRegisterObject(cmp.Key).AddTo(gameObject);
            }


            if (!onlyExplicitComponents)
            {
                foreach (var cmp in GetComponents<Component>().Except(components.Select(x => (Component)x.Object)))
                {
                    if (cmp == this)
                        continue;

                    if (cmp.SavingSystemIsInstanceRegistered())
                        continue;

                    if (cmp.SavingSystemIsTypeRegistered())
                        cmp.SavingSystemRegisterComponent().AddTo(gameObject);
                }
            }

            Destroy(this);
        }
    }
}
