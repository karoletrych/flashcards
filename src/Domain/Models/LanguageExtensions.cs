using System;

namespace Flashcards.Models
{
    public static class LanguageExtensions
    {
        public static string Tag(this Language language)
        {
            switch (language)
            {
                case Language.German:
                    return "de";
                case Language.English:
                    return "en";
                case Language.Polish:
                    return "pl";
                case Language.French:
                    return "fr";
                case Language.Italian:
                    return "it";
                case Language.Russian:
                    return "ru";
	            case Language.Norwegian:
		            return "no";
				default:
                    throw new ArgumentException($"{language}");
            }
        }

        public static Language ToLanguageEnum(this string name)
        {
            Enum.TryParse(name, out Language language);
            return language;
        }
    }
}