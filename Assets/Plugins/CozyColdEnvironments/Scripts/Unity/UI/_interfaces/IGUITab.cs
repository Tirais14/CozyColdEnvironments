using CCEnvs.FuncLanguage;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IGUITab : IShowable
    {
        Maybe<Image> image { get; }

        Maybe<IGUITab> GetParentGui();

        bool IsGuiChildOf(IGUITab guiTab);
    }
}
