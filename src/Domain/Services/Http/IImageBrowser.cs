using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Flashcards.Models;
using Newtonsoft.Json;

namespace Flashcards.Services.Http
{
    public interface IImageBrowser
    {
	    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
	    /// <exception cref="JsonReaderException">
	    ///                 <paramref name="json" /> is not valid JSON.
	    ///             </exception>
		Task<IList<Uri>> FindAsync(string query, Language queryLanguage, int numberOfResults);
    }
}