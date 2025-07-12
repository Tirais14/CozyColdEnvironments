using System;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace UTIRLib.DB
{
    public readonly struct LoadSceneInfo
    {
        public readonly string sceneName;
        public readonly LoadSceneMode loadMode;
        public readonly bool activateOnLoad;
        public readonly int priority;
        public readonly SceneReleaseMode releaseMode;

        public LoadSceneInfo(string sceneName, LoadSceneMode loadMode, bool activateOnLoad, int priority,
            SceneReleaseMode releaseMode)
        {
            this.sceneName = sceneName;
            this.loadMode = loadMode;
            this.activateOnLoad = activateOnLoad;
            this.priority = Math.Abs(priority);
            this.releaseMode = releaseMode;
        }

        public LoadSceneInfo(string sceneName, LoadSceneMode loadMode, bool activateOnLoad, int priority) :
            this(sceneName, loadMode, activateOnLoad, priority, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded)
        {
            this.sceneName = sceneName;
            this.loadMode = loadMode;
            this.activateOnLoad = activateOnLoad;
            this.priority = Math.Abs(priority);
        }

        public LoadSceneInfo(string sceneName, LoadSceneMode loadMode, bool activateOnLoad) :
            this(sceneName, loadMode, activateOnLoad, priority: 100, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded)
        {
            this.sceneName = sceneName;
            this.loadMode = loadMode;
            this.activateOnLoad = activateOnLoad;
        }

        public LoadSceneInfo(string sceneName, LoadSceneMode loadMode) :
            this(sceneName, loadMode, activateOnLoad: true, priority: 100, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded)
        {
            this.sceneName = sceneName;
            this.loadMode = loadMode;
        }

        public LoadSceneInfo(string sceneName) :
            this(sceneName, LoadSceneMode.Single, activateOnLoad: true, priority: 100,
                SceneReleaseMode.ReleaseSceneWhenSceneUnloaded)
        {
            this.sceneName = sceneName;
        }
    }
}