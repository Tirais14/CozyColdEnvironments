using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public delegate byte[] BytesCompressor(byte[] bytes, object? state);
}
