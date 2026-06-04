#nullable enable
namespace CCEnvs.UnityX.ECS
{
    /// <summary>
    /// HasValue is false if Value == 0
    /// </summary>
    public struct MaybeUnmanagedInt
    {
        public static MaybeUnmanagedInt Null => new() { Value = 0 };

        public int Value;

        /// <summary>
        /// is false if Value == 0
        /// </summary>
        public readonly bool HasValue => Value == 0;

        public static implicit operator MaybeUnmanagedInt(int? input)
        {
            if (!input.HasValue)
                return Null;

            return new MaybeUnmanagedInt { Value = input.Value };
        }

        public static implicit operator MaybeUnmanagedInt(int value)
        {
            return new MaybeUnmanagedInt { Value = value };
        }
    }
}
