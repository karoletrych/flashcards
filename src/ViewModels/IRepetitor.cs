using System.Collections.Generic;
using System.Threading.Tasks;
using Flashcards.Models;
using Prism.Navigation;

namespace Flashcards.ViewModels
{
    public interface IRepetitor
    {
        Task Repeat(
	        INavigationService navigationService, 
	        string askingQuestionsPageRelativeUri, 
	        ICollection<Flashcard> flashcardsToAsk);
    }
}