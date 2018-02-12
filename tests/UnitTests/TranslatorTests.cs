﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashCards.Models;
using FlashCards.Models.Dto;
using Xunit;

namespace FlashCards.UnitTests
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
            Assert.Equal(expectedList, _yandexTranslator.Translate(@from, to, text).Result);
        }
    }
}
