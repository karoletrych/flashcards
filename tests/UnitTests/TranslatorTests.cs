using System.Collections.Generic;
using System.Linq;
using Flashcards.Models;
using Flashcards.Services;
using Flashcards.Services.Http;
using Xunit;

namespace Flashcards.UnitTests
{
    public class TranslatorTests
    {
        private readonly YandexTranslatorService _yandexTranslatorService;

        public TranslatorTests()
        {
            _yandexTranslatorService = new YandexTranslatorService();
        }

        [Theory]
        [InlineData(Language.Polish, Language.English, "kot", new[] {"cat"})]
        [InlineData(Language.Polish, Language.English, "pies", new[]{"dog"})]
        [InlineData(Language.English, Language.Polish, "can", new[]{"może"})]
        public void Translation(Language @from, Language to, string text, string[] expected)
        {
            var expectedList = expected.ToList().AsReadOnly();
            Assert.Equal(expectedList, _yandexTranslatorService.Translate(@from, to, text).Result);
        }

        [Fact]
        public void TranlatiionOfEmptyStringReturnEmptyString()
        {
            var emptyList = new List<string>();
            Assert.Equal(emptyList, _yandexTranslatorService.Translate(Language.English, Language.French, "").Result);
        }
    }
}
