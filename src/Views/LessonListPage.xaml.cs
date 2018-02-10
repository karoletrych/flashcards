using FlashCards.ViewModels.Lesson;
using Xamarin.Forms;

namespace FlashCards.Views
{
    public partial class LessonListPage : ContentPage
    {
        public LessonListPage()
        {
            InitializeComponent();
        }

        private async void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var lesson = (LessonViewModel) e.Item;
            await Navigation.PushAsync(new NavigationPage(new LessonDetailsPage(lesson)));
        }
    }
}
