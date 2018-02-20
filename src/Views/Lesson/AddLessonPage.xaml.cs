using System;
using FlashCards.Models;
using FlashCards.ViewModels.Lesson;
using Xamarin.Forms;

namespace FlashCards.Views.Lesson
{
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

        private async void AddFlashCards_OnClicked(object sender, EventArgs e)
        {
            var viewModel = (AddLessonViewModel) BindingContext;
            var addFlashCardPage = _addFlashCardPageFactory(
                viewModel.SelectedFrontLanguage.ToLanguageEnum(),
                viewModel.SelectedBackLanguage.ToLanguageEnum());

            await Navigation.PushModalAsync(new NavigationPage(addFlashCardPage));
        }
    }
}