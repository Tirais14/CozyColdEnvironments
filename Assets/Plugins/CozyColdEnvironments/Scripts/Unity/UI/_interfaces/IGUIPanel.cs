using CCEnvs.FuncLanguage;
using UnityEngine.UI;

namespace CCEnvs.Unity.UI
{
    public interface IGUIPanel : IShowable
    {
        Maybe<Image> image { get; }
    }
}
