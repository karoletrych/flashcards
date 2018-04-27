using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;
using Flashcards.Services.Http;
using Xunit;

namespace Flashcards.ServicesTests
{
    public class TranslatorTests
    {
        private readonly YandexTranslator _yandexTranslator;

        public TranslatorTests()
        {
            _yandexTranslator = new YandexTranslator();
        }

        [Theory]
        [InlineData(Language.Polish, Language.English, "kot", new[] {"cat"})]
        [InlineData(Language.Polish, Language.English, "pies", new[]{"dog"})]
        [InlineData(Language.English, Language.Polish, "can", new[]{"może"})]
        public void Translation(Language @from, Language to, string text, string[] expected)
        {
            var expectedList = expected.ToList().AsReadOnly();
            Assert.Equal(expectedList, _yandexTranslator.TranslateAsync(@from, to, text).Result);
        }

        [Fact]
        public void TranslationOfEmptyStringReturnEmptyString()
        {
            var emptyList = new List<string>();
            Assert.Equal(emptyList, _yandexTranslator.TranslateAsync(Language.English, Language.French, "").Result);
        }
    }
}
