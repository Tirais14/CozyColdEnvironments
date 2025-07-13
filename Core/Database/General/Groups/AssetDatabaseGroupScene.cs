using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

#nullable enable

namespace UTIRLib.DB
{
    public class AssetDatabaseGroupScene : DatabaseGroup<string, AssetDatabaseScene>
    {
        public AssetDatabaseGroupScene()
        { }

        public AssetDatabaseGroupScene(string databaseKey, AssetDatabaseScene database) =>
            AddDatabase(databaseKey, database);

        public AssetDatabaseGroupScene(IValuePair<string, AssetDatabaseScene>[] keyAndDatabases) =>
            AddDatabaseRange(keyAndDatabases);

        public async Task<SceneInstance> LoadSceneAsync(string databaseKey, string sceneName)
        {
            AssetDatabaseScene? database = GetDatabase(databaseKey);
            if (database == null)
            {
                return default;
            }

            return await database.LoadSceneAsync(new LoadSceneInfo(sceneName));
        }

        public async Task<SceneInstance> LoadSceneAsync(string databaseKey, LoadSceneInfo loadSceneInfo)
        {
            AssetDatabaseScene? database = GetDatabase(databaseKey);
            if (database == null)
            {
                return default;
            }

            return await database.LoadSceneAsync(loadSceneInfo);
        }

        public async Task<SceneInstance> LoadSceneAsync(string databaseKey, string sceneName, LoadSceneMode loadMode,
            bool activateOnLoad, int priority, SceneReleaseMode releaseMode)
        {
            AssetDatabaseScene? database = GetDatabase(databaseKey);
            if (database == null)
            {
                return default;
            }

            return await database.LoadSceneAsync(new LoadSceneInfo(sceneName, loadMode, activateOnLoad, priority, releaseMode));
        }
    }
}