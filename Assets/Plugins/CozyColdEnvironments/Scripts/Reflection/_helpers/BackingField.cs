using CommunityToolkit.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class BackingField
    {
        public static string HumanizeName(FieldInfo field)
        {
            Guard.IsNotNull(field, nameof(field));

            var match = Regex.Match(field.Name, @"^(<(?:\w+)>)k__BackingField$");

            if (!match.Success)
                return field.Name;

            return match.Groups[1].Value;
        }
    }
}
