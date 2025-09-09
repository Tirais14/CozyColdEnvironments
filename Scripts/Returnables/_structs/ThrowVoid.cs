#nullable enable
#pragma warning disable
namespace CCEnvs.Returnables
{
    public readonly struct ThrowVoid
    {
        /// <summary>
        /// It is mock method for conversations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public object AsObject() => this;

        /// <summary>
        /// It is mock method for conversations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>() => default!;
    }
}
