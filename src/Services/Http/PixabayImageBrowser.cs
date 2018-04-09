using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flashcards.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Flashcards.Services.Http
{
    public class PixabayImageBrowser : IImageBrowser
    {
        private const string PixabayKey = "7086795-ed8c5c96965624c739c6e22af";
        private readonly HttpClient _httpClient = new HttpClient();

	    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	    /// <exception cref="JsonReaderException">
	    ///                 <paramref name="json" /> is not valid JSON.
	    ///             </exception>
	    public async Task<IList<Uri>> Find(string query, Language queryLanguage, int numberOfResults = 9)
        {
            var httpQuery =
                new Uri("https://pixabay.com/api/?")
                    .AddQuery("key", PixabayKey)
                    .AddQuery("q", query)
                    // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                    .AddQuery("per_page", numberOfResults.ToString())
                    .AddQuery("lang", queryLanguage.Tag())
                    .ToString();

            var response = await _httpClient.GetStringAsync(httpQuery);

            dynamic json = JObject.Parse(response);
            var uris = ((IEnumerable<dynamic>) json.hits)
                .Select(hit => new Uri((string)hit.previewURL))
	            .ToList();
            return uris;
        }
    }
}