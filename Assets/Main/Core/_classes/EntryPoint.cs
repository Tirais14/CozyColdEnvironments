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
