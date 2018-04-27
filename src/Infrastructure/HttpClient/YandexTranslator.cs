using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flashcards.Models;
using Newtonsoft.Json;

namespace Flashcards.Services.Http
{
    public class YandexTranslator : ITranslator
    {
        private const string YandexKey =
            "trnsl.1.1.20171117T191335Z.d621ce719bfba7b1.078c313c6f9536f1a4cb15469216f22f05a2318b";


        private readonly HttpClient _client = new HttpClient();

	    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	    public async Task<IEnumerable<string>> TranslateAsync(Language from, Language to, string text)
        {
            if (!text.Any())
                return new List<string>();

            var request = new Uri("https://translate.yandex.net/api/v1.5/tr.json/translate")
                .AddQuery("key", YandexKey)
                .AddQuery("text", text)
                .AddQuery("lang", $"{@from.Tag()}-{to.Tag()}")
                .ToString();

            var response = await _client.GetStringAsync(request);

            var json = JsonConvert.DeserializeObject<dynamic>(response);
            var translations = json["text"].ToObject<IEnumerable<string>>();

            return translations;
        }
    }
}