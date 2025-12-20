using CCEnvs.Collections;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Saves;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Linq;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [RequireComponent(typeof(PersistentGuid))]
    public sealed class SavingSystemRegistrationComponent : CCBehaviour
    {
        public SavingSystemToRegisterObject[] components = Array.Empty<SavingSystemToRegisterObject>();

        [Tooltip("Always false when component list is empty. Better keep true, otherwise may cause key duplicates and restoring will become impossible or incorrect")]
        public bool onlyExplicitComponents = true;

        protected override void Start()
        {
            base.Start();

            gameObject.SavingSystemRegisterUnityObject().AddTo(gameObject);

            if (components.IsNotNullOrEmpty())
            {
                foreach (var cmp in components)
                    cmp.Object.SavingSystemRegisterObject(cmp.Key).AddTo(gameObject);
            }

            if (!onlyExplicitComponents || components.IsNullOrEmpty())
            {
                foreach (var cmp in GetComponents<Component>().Except(components.Select(x => (Component)x.Object)))
                {
                    if (cmp == this)
                        continue;

                    if (cmp.SavingSystemIsInstanceRegistered())
                        continue;

                    if (cmp.SavingSystemIsTypeRegistered())
                        cmp.SavingSystemRegisterUnityObject().AddTo(gameObject);
                }
            }

            Destroy(this);
        }
    }
}
