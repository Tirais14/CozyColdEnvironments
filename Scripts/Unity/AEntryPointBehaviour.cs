using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public abstract class AEntryPointBehaviour : CCBehaviourStatic
    {
        private int frames;

        private void Update()
        {
            if (frames >= 10)
                Destroy(gameObject);

            frames++;
        }
    }
}
