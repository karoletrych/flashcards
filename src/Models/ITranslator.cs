using System.Collections.Generic;
using System.Threading.Tasks;
using FlashCards.Models.Dto;

namespace FlashCards.Models
{
    public interface ITranslator
    {
        Task<IReadOnlyList<string>> Translate(Language @from, Language to, string text);
    }
}