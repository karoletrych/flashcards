using FlashCards.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlashCards.Services
{
    public interface ITranslator
    {
        Task<IEnumerable<string>> Translate(Language @from, Language to, string text);
    }
}