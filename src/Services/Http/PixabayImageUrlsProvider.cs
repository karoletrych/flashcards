using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flashcards.Models;
using Newtonsoft.Json.Linq;

namespace Flashcards.Services.Http
{
    public class PixabayImageUrlsProvider : IImageUrlsProvider
    {
        private const string PixabayKey = "7086795-ed8c5c96965624c739c6e22af";
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly int _numberOfResults;

        public PixabayImageUrlsProvider(int numberOfResults = 9)
        {
            _numberOfResults = numberOfResults;
        }

        public async Task<IEnumerable<string>> Find(string query, Language queryLanguage)
        {
            var httpQuery =
                new Uri("https://pixabay.com/api/?")
                    .AddQuery("key", PixabayKey)
                    .AddQuery("q", query)
                    // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                    .AddQuery("per_page", _numberOfResults.ToString())
                    .AddQuery("lang", queryLanguage.Acronym())
                    .ToString();

            var response = await _httpClient.GetStringAsync(httpQuery);

            dynamic json = JObject.Parse(response);
            var urls = ((IEnumerable<dynamic>) json.hits)
                .Select(hit => (string)hit.previewURL);
            return urls;
        }
    }
}