using CCEnvs.FuncLanguage;
using UnityEngine.UI;


namespace CCEnvs.Unity.UI
{
    public interface IViewElement : IShowable
    {
        Maybe<Image> Img { get; }
    }
}
