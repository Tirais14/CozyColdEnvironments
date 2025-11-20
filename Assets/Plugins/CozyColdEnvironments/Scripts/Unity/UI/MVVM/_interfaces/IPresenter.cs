#nullable enable
#pragma warning disable IDE1006
using System.Runtime.CompilerServices;

namespace CCEnvs.Unity.UI.MVVM
{
    public interface IPresenter
    {
        object model { get; }

        //void ForceNotify();
    }
    public interface IPresenter<out TModel> : IPresenter
    {
        new TModel model { get; }

        object IPresenter.model => model!;
    }
}
