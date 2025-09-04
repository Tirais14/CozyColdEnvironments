using System;

#nullable enable
namespace CCEnvs
{
    public static class DelegateExtensions
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Contains(this Delegate? value, Delegate other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));
            if (value is null)
                return false;
            if (value == other)
                return true;

            Delegate[] invocationList = value.GetInvocationList();

            int count = invocationList.Length;
            for (int i = 0; i < count; i++)
            {
                if (invocationList[i].Method == other.Method)
                    return true;
            }

            return false;
        }
    }
}
