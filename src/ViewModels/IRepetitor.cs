using System.Threading.Tasks;
using Flashcards.Services.Examiner;
using Prism.Navigation;

namespace Flashcards.ViewModels
{
    public interface IRepetitor
    {
        Task Repeat(
	        INavigationService navigationService, 
	        string askingQuestionsPageRelativeUri, 
	        IExaminer examiner);
    }
}