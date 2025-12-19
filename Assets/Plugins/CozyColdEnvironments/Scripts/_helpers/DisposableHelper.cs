using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

namespace CCEnvs
{
    public static class DisposableHelper
    {
        public static void DisposeEach(this IEnumerable<IDisposable> source)
        {
            Guard.IsNotNull(source, nameof(source));

            foreach (var item in source)
                item.Dispose();
        }
    }
}
