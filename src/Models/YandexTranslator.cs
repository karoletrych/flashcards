﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FlashCards.Models.Dto;
using Newtonsoft.Json;

namespace FlashCards.Models
{
    public class YandexTranslator : ITranslator
    {
        private const string YandexKey =
            "trnsl.1.1.20171117T191335Z.d621ce719bfba7b1.078c313c6f9536f1a4cb15469216f22f05a2318b";


        private readonly HttpClient _client = new HttpClient();

        public async Task<IReadOnlyList<string>> Translate(Language from, Language to, string text)
        {
            var request = new Uri("https://translate.yandex.net/api/v1.5/tr.json/translate")
                .AddQuery("key", YandexKey)
                .AddQuery("text", text)
                .AddQuery("lang", $"{@from.Acronym()}-{to.Acronym()}")
                .ToString();

            var response = await _client.GetStringAsync(request);

            var json = JsonConvert.DeserializeObject<dynamic>(response);
            var translations = json["text"].ToObject<IReadOnlyList<string>>();

            return translations;
        }
    }
}