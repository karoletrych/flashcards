using Xamarin.Forms;

namespace FlashCards.Views.Lesson
{
    public partial class LessonListPage : ContentPage
    {
        public LessonListPage()
        {
            InitializeComponent();
        }

        private async void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
//            var lesson = (LessonViewModel) e.Item;
//            var lessonDetailsPage = _lessonDetailsPageFactory(lesson);
//            await Navigation.PushAsync(new NavigationPage(lessonDetailsPage));
        }
    }
}