using CCEnvs.FuncLanguage;
using UnityEngine.UI;

namespace CCEnvs.Unity.UI
{
    public interface IGUITab : IShowable
    {
        Maybe<Image> image { get; }
    }
}
