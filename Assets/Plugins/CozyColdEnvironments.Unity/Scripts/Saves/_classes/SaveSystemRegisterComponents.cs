using System.Collections.Generic;
using CCEnvs.Collections;
using CCEnvs.Saves;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public sealed class SaveSystemRegisterComponents : MonoBehaviour
    {
        private List<SaveGroupRegistration>? registrations;

        [field: SerializeField]
        public Component[] Components { get; set; } = new arr<Component>();

        [field: SerializeField]
        public string GroupName { get; set; } = null!;

        [field: SerializeField]
        public string CatalogPath { get; set; } = null!;

        [field: SerializeField]
        public string ArchivePath { get; set; } = null!;

        [field: SerializeField]
        public bool Incremental { get; set; }

        private void Start()
        {
            if (Components.IsNotNullOrEmpty())
                return;

            var saveGroup = SaveSystem.GetOrCreateArchive(ArchivePath)
                .GetOrCreateCatalog(CatalogPath)
                .GetOrCreateGroupGeneric(GroupName, Incremental);

            registrations = new List<SaveGroupRegistration>(Components.Length);

            foreach (var cmp in Components)
            {
                try
                {
                    var reg = saveGroup.RegisterObjectHandled(cmp, cmp.GetExtraInfo().ToString());

                    registrations.Add(reg);
                }
                catch (System.Exception ex)
                {
                    this.PrintException(ex);
                }
            }

            if (registrations.IsEmpty())
                registrations = null;
            else
                registrations.TrimExcess();
        }

        private void OnDestroy()
        {
            if (registrations is not null)
            {
                foreach (var reg in registrations)
                    reg.Dispose();
            }
        }
    }
}
