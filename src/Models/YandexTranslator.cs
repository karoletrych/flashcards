using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using FlashCards.Models.Dto;
using Newtonsoft.Json;

namespace FlashCards.Models
{
    public class YandexTranslator : ITranslator
    {
        private const string YandexKey =
            "trnsl.1.1.20171117T191335Z.d621ce719bfba7b1.078c313c6f9536f1a4cb15469216f22f05a2318b";


        private readonly HttpClient _client = new HttpClient();

        private string Acronym(Language language)
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

        public async Task<IReadOnlyList<string>> Translate(Language from, Language to, string text)
        {
            var request = new Uri("https://translate.yandex.net/api/v1.5/tr.json/translate")
                .AddQuery("key", YandexKey)
                .AddQuery("text", text)
                .AddQuery("lang", $"{Acronym(from)}-{Acronym(to)}")
                .ToString();

            var response = await _client.GetAsync(request);

            var serializedResponse = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<dynamic>(serializedResponse);
            var translations = json["text"].ToObject<List<string>>();

            return translations;
        }
    }

    static class HttpExtensions
    {
        public static Uri AddQuery(this Uri uri, string name, string value)
        {
            var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);

            var ub = new UriBuilder(uri) { Query = httpValueCollection.ToString() };

            return ub.Uri;
        }
    }
}