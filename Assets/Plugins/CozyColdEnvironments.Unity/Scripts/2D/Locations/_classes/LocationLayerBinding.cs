using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
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
