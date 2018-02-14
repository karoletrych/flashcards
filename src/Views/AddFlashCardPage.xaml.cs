using System;
using FlashCards.Models;
using FlashCards.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlashCards.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddFlashCardPage : ContentPage
    {
        public delegate AddFlashCardPage Factory(Language frontLanguage, Language backLanguage);

        public AddFlashCardPage(
            Language frontLanguage,
            Language backLanguage,
            AddFlashCardViewModel.Factory addFlashCardViewModelFactory)
        {
            BindingContext = addFlashCardViewModelFactory(frontLanguage, backLanguage);
            InitializeComponent();
            GenerateImagesGrid();
        }

        private void FrontEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!((Entry) sender).IsFocused)
                return; // do not translate when entry was not modified by user
            var viewModel = (AddFlashCardViewModel) BindingContext;
            viewModel.FrontTextModified();
        }

        private void BackEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!((Entry) sender).IsFocused)
                return; // do not translate when entry was not modified by user
            var viewModel = (AddFlashCardViewModel) BindingContext;
            viewModel.BackTextModified();
        }

        private void GenerateImagesGrid()
        {
            const int columns = 3;
            const int rows = 3;
            for (var i = 0; i < columns; ++i)
                ImagesGrid.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width = GridLength.Star
                    });
            for (var j = 0; j < rows; ++j)
                ImagesGrid.RowDefinitions.Add(
                    new RowDefinition
                    {
                        Height = GridLength.Star
                    });

            for (var i = 0; i < columns; ++i)
            for (var j = 0; j < rows; ++j)
                ImagesGrid.Children.Add(new BoxView {Color = Color.Green}, i, j);
        }

        private void YandexLabel_OnTapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("http://translate.yandex.com"));
        }
    }
}