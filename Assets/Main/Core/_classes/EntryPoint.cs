using CCEnvs;
using CCEnvs.Saves;
using CCEnvs.Unity.Components;
using UnityEngine.SceneManagement;

#nullable enable
namespace Core
{
    public class EntryPoint : CCBehaviour
    {
        protected override void Awake()
        {
            base.Awake();

            CC.Install(Range.From("Tests*", "Core*"));

            InstallSaveSystem();
        }

        protected override void Start()
        {
            base.Start();

            SceneManager.LoadScene(1);
        }

        private void InstallSaveSystem()
        {
            SaveSystem.GetOrCreateArchive(Saves.GetArchivePath(Saves.SETTINGS_ARCHIVE_NAME))
                .GetOrCreateCatalog(Saves.SETTINGS_CATALOG_PATH)
                .GetOrCreateIncrementalGroup(Saves.SETTINGS_GROUP_NAME);

            var saveArchive = SaveSystem.GetOrCreateArchive(Saves.GetArchivePath(Saves.SAVE_ARCHIVE_NAME));

            var playerCatalog = saveArchive.GetOrCreateCatalog(Saves.PLAYER_CATALOG_PATH);

            playerCatalog.CreateIncrementalGroup(Saves.DefaultCreateGroupParams.WithGroupName(Saves.PLAYER_GROUP_NAME));
        }
    }
}
