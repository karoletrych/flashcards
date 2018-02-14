using System;
using FlashCards.Models;
using FlashCards.ViewModels.Lesson;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlashCards.Views.Lesson
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class AddLessonPage : ContentPage
    {
        private readonly AddFlashCardPage.Factory _addFlashCardPageFactory;

        public AddLessonPage(AddLessonViewModel addLessonViewModel,
            AddFlashCardPage.Factory addFlashCardPageFactory)
        {
            _addFlashCardPageFactory = addFlashCardPageFactory;
            InitializeComponent();
            BindingContext = addLessonViewModel;
        }

        private async void MenuItem_OnClicked(object sender, EventArgs e)
        {
            var viewModel = (AddLessonViewModel) BindingContext;
            var addFlashCardPage = _addFlashCardPageFactory(
                viewModel.SelectedFrontLanguage.ToLanguageEnum(),
                viewModel.SelectedBackLanguage.ToLanguageEnum());

            await Navigation.PushModalAsync(new NavigationPage(addFlashCardPage));
        }
    }
}