using CommunityToolkit.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class BackingField
    {
        public static string NameRegex { get; } = @"^(<(?:\w+)>)k__BackingField$";

        public static string HumanizeName(FieldInfo field)
        {
            Guard.IsNotNull(field, nameof(field));

            var match = Regex.Match(field.Name, NameRegex);

            if (!match.Success)
                return field.Name;

            return match.Groups[1].Value;
        }

        public static bool IsBackingField(this FieldInfo field)
        {
            return Regex.Match(field.Name, BackingField.NameRegex).Success;
        }
    }
}
