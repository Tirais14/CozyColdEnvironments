using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    public sealed class SaveableMarker : CCBehaviour
    {
        protected override void Start()
        {
            base.Start();

            foreach (var sub in gameObject.BindComponentsToSaveSystem())
                sub.AddTo(gameObject);

            Destroy(this);
        }
    }
}
