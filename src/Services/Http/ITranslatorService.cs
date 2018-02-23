using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.Models;

namespace Flashcards.Services.Http
{
    public interface ITranslatorService
    {
        Task<IEnumerable<string>> Translate(Language @from, Language to, string text);
    }
}