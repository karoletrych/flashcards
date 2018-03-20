using System.Threading.Tasks;
using Prism.Navigation;

namespace Flashcards.ViewModels
{
    public interface IRepetition
    {
        Task Repeat(INavigationService navigationService);
    }
}