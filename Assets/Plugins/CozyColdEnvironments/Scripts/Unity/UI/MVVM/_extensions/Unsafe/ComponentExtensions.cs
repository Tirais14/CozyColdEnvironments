using CCEnvs.Reflection;

#nullable enable
namespace CCEnvs.Unity.UI.MVVM.Unsafe
{
    public static class ComponentExtensions
    {
        public static void SetModel(this IViewModel viewModel, object model)
        {
            viewModel.AsReflected().Field(nameof(IViewModel.model)).SetValue(model);
        }
    }
}
