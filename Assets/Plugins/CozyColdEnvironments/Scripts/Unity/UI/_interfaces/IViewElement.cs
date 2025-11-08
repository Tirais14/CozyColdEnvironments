using CCEnvs.FuncLanguage;
using UnityEngine.UI;


namespace CCEnvs.Unity.UI
{
    public interface IViewElement : IShowable, ISelectable
    {
        Maybe<Image> image { get; }
    }
}
