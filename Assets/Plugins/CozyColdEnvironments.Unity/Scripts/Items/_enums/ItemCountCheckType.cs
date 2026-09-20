#nullable enable
namespace CCEnvs.UnityX.Items
{
    public enum ItemCountCheckType
    {
        None,
        BiggerOrEquals,
        Equals,
        Default = BiggerOrEquals
    }

    public static class ItemCountCheckTypeExtensions
    {
        public static bool IsMatch(this ItemCountCheckType source, int left, int right)
        {
            return source switch
            {
                ItemCountCheckType.BiggerOrEquals => left >= right,
                ItemCountCheckType.Equals => left == right,
                _ => throw CC.ThrowHelper.InvalidOperationException(source),
            };
        }
        public static bool IsMatch(this ItemCountCheckType source, long left, long right)
        {
            return source switch
            {
                ItemCountCheckType.BiggerOrEquals => left >= right,
                ItemCountCheckType.Equals => left == right,
                _ => throw CC.ThrowHelper.InvalidOperationException(source),
            };
        }
    }
}
