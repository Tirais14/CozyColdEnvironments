using CCEnvs.FuncLanguage;

#nullable enable
namespace CCEnvs.UnityX._2D.Locations
{
    public interface ILocationLayerBinding
    {
        Maybe<ILocationLayer> LocationLayer { get; }

        void BindLocationLayer(ILocationLayer? layer);
    }
}
