using System;
using System.Collections.Generic;

#nullable enable
namespace UTIRLib.Disposables
{
    public interface IDisposableCollection : ICollection<IDisposable>, IDisposable
    {   
        
    }
}
