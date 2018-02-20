using System.Collections.Generic;
using System.Threading.Tasks;
using FlashCards.Models;

namespace FlashCards.Services.Http
{
    public interface ITranslator
    {
        Task<IEnumerable<string>> Translate(Language @from, Language to, string text);
    }
}