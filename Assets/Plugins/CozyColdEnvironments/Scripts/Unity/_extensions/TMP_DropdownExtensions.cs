using CCEnvs.FuncLanguage;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using TMPro;

#nullable enable
namespace CCEnvs.Unity
{
    public static class TMP_DropdownExtensions
    {
        public static (TMP_Dropdown.OptionData value, int numValue) GetOption(
            this TMP_Dropdown source,
            string optionText,
            bool ignoreCase = false)
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(optionText, nameof(optionText));

            var optCount = source.options.Count;

            for (int i = 0; i < optCount; i++)
            {
                if (source.options[i].text.EqualsOrdinal(optionText, ignoreCase))
                    return (source.options[i], i);
            }

            throw new System.InvalidOperationException($"Option: {optionText} doesn't exists");
        }

        public static int SetOption(
           this TMP_Dropdown source,
           string optionText,
           bool ignoreCase = false)
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(optionText, nameof(optionText));

            var option = source.GetOption(optionText, ignoreCase);
            source.value = option.numValue;

            return option.numValue;
        }

        public static void Fill(
            this TMP_Dropdown source,
            params string[] values)
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(values, nameof(values));

            source.ClearOptions();

            foreach (var value in values)
                source.options.Add(new TMP_Dropdown.OptionData(value));
        }
    }
}
