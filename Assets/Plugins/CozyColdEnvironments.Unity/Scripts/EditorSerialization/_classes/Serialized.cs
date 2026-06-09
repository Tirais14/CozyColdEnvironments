using CCEnvs.Attributes;
using System;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable S3459
namespace CCEnvs.UnityX.EditorSerialization
{
    [Serializable]
    public abstract class Serialized<TOut>
        : IEditorSerialized<TOut>,
        IMutableType<TOut>
    {
        protected readonly Lazy<TOut> lazy;

        public TOut Data {
            [Converter]
            get => lazy.Value;
        }

        protected Serialized()
        {
            lazy = new Lazy<TOut>(ValueFactory);
        }

        protected Serialized(TOut defaultValue)
        {
            lazy = new Lazy<TOut>(defaultValue);
        }

        public static implicit operator TOut(Serialized<TOut> source)
        {
            return source.Data;
        }

        protected abstract TOut ValueFactory();

        TOut IMutableType<TOut>.MutateType() => Data;
    }
}
