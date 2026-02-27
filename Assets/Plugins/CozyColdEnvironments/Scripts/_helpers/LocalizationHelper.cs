#nullable enable
namespace CCEnvs
{
    public class LocalizationHelper
    {
        public static string GetTranslatedLocaleName(string locale)
        {
            return locale switch
            {
                "Russian (ru)" => "Русский (ru)",
                "English (en)" => locale,
                _ => throw new System.NotSupportedException($"Locale: {locale} not supported")
            };
        }

        public static string GetRawLocaleName(string translatedLocale)
        {
            return translatedLocale switch
            {
                "Русский (ru)" => "Russian (ru)",
                "English (en)" => translatedLocale,
                _ => throw new System.NotSupportedException($"Locale: {translatedLocale} not supported")
            };
        }
    }
}
