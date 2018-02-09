using ViewModels;
using Xamarin.Forms;

namespace Views
{
    class QuestionsPageService : INavigationService
    {
        public void NavigateTo(object context)
        {
            var questionsPage = new NavigationPage(new AskingQuestionsPage());
            questionsPage.BindingContext = context;
        }
    }
}
