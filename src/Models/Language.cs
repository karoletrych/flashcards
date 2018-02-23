using System;

namespace Flashcards.Models
{
    public enum Language
    {
        German,
        English,
        Polish,
        French,
        Italian,
        Spanish,
        Swedish,
        Norwegian,
        Russian
    }

    public static class LanguageExtensions
    {
        public static string Acronym(this Language language)
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
                case Language.Spanish:
                    throw new NotImplementedException();
                case Language.Swedish:
                    throw new NotImplementedException();
                case Language.Norwegian:
                    throw new NotImplementedException();
                case Language.Russian:
                    return "ru";
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