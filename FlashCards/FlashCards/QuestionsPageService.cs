using FlashCards.ViewModel;
using Xamarin.Forms;

namespace FlashCards
{
    class QuestionsPageService : INavigationService
    {
        public void NavigateTo(object context)
        {
            var questionsPage = new NavigationPage(new QuestionsPage());
            questionsPage.BindingContext = context;
        }
    }
}
