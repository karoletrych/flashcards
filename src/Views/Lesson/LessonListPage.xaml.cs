using System;
using FlashCards.ViewModels.Lesson;
using Xamarin.Forms;

namespace FlashCards.Views.Lesson
{
    public partial class LessonListPage : ContentPage
    {
        private readonly Func<LessonViewModel, LessonDetailsPage> _lessonDetailsPageFactory;

        public LessonListPage(LessonListViewModel lessonListViewModel,
            Func<LessonViewModel, LessonDetailsPage> lessonDetailsPageFactory)
        {
            _lessonDetailsPageFactory = lessonDetailsPageFactory;
            InitializeComponent();

            BindingContext = lessonListViewModel;
        }

        private async void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var lesson = (LessonViewModel) e.Item;
            var lessonDetailsPage = _lessonDetailsPageFactory(lesson);
            await Navigation.PushAsync(new NavigationPage(lessonDetailsPage));
        }

        private async void MenuItem_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new AddLessonPage()));
        }
    }
}