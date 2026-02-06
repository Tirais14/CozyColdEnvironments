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
    public sealed class SavingSystemRegistrationComponent : CCBehaviour
    {
        public SavingSystemToRegisterObject[] components = Array.Empty<SavingSystemToRegisterObject>();

        [Tooltip("Always false when component list is empty. Better keep true, otherwise may cause key duplicates and restoring will become impossible or incorrect")]
        public bool onlyExplicitComponents = false;

        [Tooltip("Doesn't register parent game object")]
        public bool ignoreGameObject = true;

        protected override void Start()
        {
            base.Start();

            if (!ignoreGameObject)
                RegisterGameObject();

            if (components.IsNotNullOrEmpty())
                RegisterExplicitComponents();

            if (!onlyExplicitComponents || components.IsNullOrEmpty())
                RegisterOtherComponents();

            Destroy(this);
        }

        private void RegisterGameObject()
        {
            gameObject.SavingSystemRegisterUnityObject().AddTo(gameObject);
        }

        private void RegisterExplicitComponents()
        {
            foreach (var cmp in components)
                cmp.Object.SavingSystemRegisterObject(cmp.Key).AddTo(gameObject);
        }

        private void RegisterOtherComponents()
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
    }
}
