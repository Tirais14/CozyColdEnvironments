using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Serialization
{
    [Serializable]
    public abstract class DataTransferObject<T>
        :
        IDataTransferObject<T>
    {
        private T? data;

        private bool isDataCreated;

        protected DataTransferObject() { }

        protected DataTransferObject(T value)
        {
            data = value;
            isDataCreated = true;
        }

        public virtual T Materialize()
        {
            if (!isDataCreated)
            {
                data = CreateData();
                isDataCreated = true;
            }

            return data!;
        }

        protected abstract T CreateData();
    }

    [Serializable]
    public abstract class DataTransferObject<T, TConverted>
        :
        IDataTransferObject<TConverted>
    {
        private TConverted? data;

        private bool isDataCreated;

        private Func<T, TConverted>? converter;

        protected DataTransferObject(Func<T, TConverted> converter)
        {
            Guard.IsNotNull(converter);
            this.converter = converter;
        }

        protected DataTransferObject(TConverted value)
        {
            data = value;
            isDataCreated = true;
        }

        public virtual TConverted Materialize()
        {
            if (!isDataCreated)
            {
                if (converter is null)
                    throw new InvalidOperationException("Converter not found");

                data = converter(CreateData());
                isDataCreated = true;
                return data;
            }

            return data!;
        }

        protected abstract T CreateData();
    }
}
