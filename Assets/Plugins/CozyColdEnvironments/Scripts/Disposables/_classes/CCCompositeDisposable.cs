using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace CCEnvs.Disposables
{
    public class CCCompositeDisposable : List<IDisposable>, IDisposable
    {
        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            for (int i = 0; i < Count; i++)
            {
                try
                {
                    this[i].Dispose();
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }
            }
        }
    }
}
