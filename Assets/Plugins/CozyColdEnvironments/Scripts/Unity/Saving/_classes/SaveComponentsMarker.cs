using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using R3;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    public sealed class SaveComponentsMarker : CCBehaviour
    {
        protected override void Start()
        {
            base.Start();

            foreach (var sub in gameObject.BindComponentsToSaveSystem())
                sub.AddTo(gameObject.GetCancellationTokenOnDestroy());

            Destroy(this);
        }
    }
}
