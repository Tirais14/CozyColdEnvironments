#nullable enable

using Object = UnityEngine.Object;

namespace UTIRLib.Diagnostics
{
    public readonly struct NullValidator<T>
    {
        public readonly bool isNull;
        public readonly bool isUnityNull;

        public readonly bool AnyNull => isNull || isUnityNull;

        public NullValidator(T? obj)
        {
            if (obj is Object unityObj)
            {
                isNull = false;
                isUnityNull = unityObj == null;
            }
            else
            {
                isNull = obj is null;
                isUnityNull = false;
            }
        }

        public static implicit operator bool(NullValidator<T> nullValidationResult) => nullValidationResult.AnyNull;
    }
}