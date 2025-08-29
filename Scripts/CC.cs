#nullable enable

#pragma warning disable S101
namespace CCEnvs
{
    public static class CC
    {
        public static string WordSeparator { get; set; } = "_";

        public static class Create 
        {
            public static T[] Array<T>(params T[] values) => values;
        }
    }
}