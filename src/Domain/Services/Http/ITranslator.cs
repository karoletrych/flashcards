using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Flashcards.Models;

namespace Flashcards.Services.Http
{
    public interface ITranslator
    {
	    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		Task<IEnumerable<string>> TranslateAsync(Language @from, Language to, string text);
    }
}