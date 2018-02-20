using System.Collections.Generic;
using System.Threading.Tasks;
using FlashCards.Models;

namespace FlashCards.Services.Http
{
    public interface IImageUrlsProvider
    {
        Task<IEnumerable<string>> Find(string query, Language queryLanguage);
    }
}