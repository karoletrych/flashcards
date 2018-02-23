using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.Models;

namespace Flashcards.Services.Http
{
    public interface IImageUrlsProvider
    {
        Task<IEnumerable<string>> Find(string query, Language queryLanguage);
    }
}