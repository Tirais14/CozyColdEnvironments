#nullable enable
namespace CCEnvs
{
    public readonly ref struct Result
    {
        private readonly object value;

        public Result(object value)
        {
            this.value = value!;
        }

        public object AsObject() => value;

        public T As<T>() => (T)value;
    }
}
