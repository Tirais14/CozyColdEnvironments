using CCEnvs.FuncLanguage;
using CCEnvs.UnityX.Components;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX._2D.Locations
{
    public class LocationLayerBinding : CCBehaviour, ILocationLayerBinding
    {
        [SerializeField]
        protected LocationLayer? m_LocationLayer = null!;

        protected ILocationLayer? locationLayer;

        public Maybe<ILocationLayer> LocationLayer {
            get
            {
                if (m_LocationLayer == null)
                    return locationLayer.Maybe();

                return m_LocationLayer;
            }
        }

        public void BindLocationLayer(ILocationLayer? layer)
        {
            locationLayer = layer;
        }
    }
}
