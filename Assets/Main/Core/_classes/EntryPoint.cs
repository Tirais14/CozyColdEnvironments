using CCEnvs;
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

            CC.Install(Range.From("Tests*"));

            SceneManager.LoadScene(1);
        }
    }
}
