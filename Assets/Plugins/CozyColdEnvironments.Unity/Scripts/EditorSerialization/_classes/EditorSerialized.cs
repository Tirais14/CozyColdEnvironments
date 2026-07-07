using CCEnvs.Attributes;
using CommunityToolkit.Diagnostics;
using System;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable S3459
namespace CCEnvs.UnityX.EditorSerialization
{
    [Serializable]
    public abstract class EditorSerialized<T>
        :
        IEditorSerialized<T>,
        IMutableType<T>
    {
        [NonSerialized]
        private T? data;

        [NonSerialized]
        private bool isValueCreated;

        public T Data {
            [Converter]
            get
            {
                if (!isValueCreated)
                {
                    data = CreateValue();
                    isValueCreated = true;
                }

                return data!;
            }
        }

        protected EditorSerialized()
        {
        }

        protected EditorSerialized(T defaultValue)
        {
            data = defaultValue;
            isValueCreated = true;
        }

        public static implicit operator T(EditorSerialized<T> source)
        {
            return source.Data;
        }

        protected abstract T CreateValue();

        T IMutableType<T>.MutateType() => Data;
    }

    public abstract class EditorSerialized<T, TConverted>
        :
        IEditorSerialized<TConverted>,
        IMutableType<TConverted>
    {
        [NonSerialized]
        private TConverted? data;

        [NonSerialized]
        private bool isValueCreated;

        [NonSerialized]
        private Func<T, TConverted>? converter;

        public TConverted Data {
            [Converter]
            get
            {
                if (!isValueCreated)
                {
                    if (converter is null)
                        throw new InvalidOperationException("Converter not found");

                    data = converter(CreateValue());
                    isValueCreated = true;
                }

                return data!;
            }
        }

        protected EditorSerialized(Func<T, TConverted> converter)
        {
            Guard.IsNotNull(converter);
            this.converter = converter;
        }

        protected EditorSerialized(TConverted defaultValue)
        {
            data = defaultValue;
            isValueCreated = true;
        }

        public static implicit operator TConverted(EditorSerialized<T, TConverted> source)
        {
            return source.Data;
        }

        protected abstract T CreateValue();

        TConverted IMutableType<TConverted>.MutateType() => Data;
    }
}
