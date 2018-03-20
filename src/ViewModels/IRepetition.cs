using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.Models;
using Prism.Navigation;

namespace Flashcards.ViewModels
{
    public interface IRepetition
    {
        Task Repeat(INavigationService navigationService, IEnumerable<Flashcard> flashcardsToAsk);
    }
}