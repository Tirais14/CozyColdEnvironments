using System;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.UI
{
    public static class DisposableExtensions
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static IDisposable AddTo(this IDisposable value, IViewModel viewModel)
        {
            if (value.IsNull())
                throw new ArgumentNullException(nameof(value));
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            viewModel.BindDisposable(value);

            return value;
        }
    }
}
