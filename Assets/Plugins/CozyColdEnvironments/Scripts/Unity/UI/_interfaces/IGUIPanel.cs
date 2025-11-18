using CCEnvs.FuncLanguage;
using UnityEngine.UI;

namespace CCEnvs.Unity.UI
{
    public interface IGUIPanel : IShowable, ISelectable
    {
        Maybe<Image> image { get; }
    }
}
