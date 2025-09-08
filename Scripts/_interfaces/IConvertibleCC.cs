#nullable enable
using System;

namespace CCEnvs
{
    public interface IConvertibleCC
    {
        object Convert();
    }
    public interface IConvertibleCC<out T> : IConvertibleCC
    {
        new T Convert();

        object IConvertibleCC.Convert() => Convert()!;
    }
}
