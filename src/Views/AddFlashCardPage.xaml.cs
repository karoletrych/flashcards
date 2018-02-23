using System;
using Flashcards.ViewModels;
using Xamarin.Forms;

namespace Flashcards.Views
{
    public partial class AddFlashcardPage : ContentPage
    {
        public AddFlashcardPage()
        {
            InitializeComponent();
            GenerateImagesGrid();
        }

        private void FrontEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!((Entry) sender).IsFocused)
                return; // do not translate when entry was not modified by user
            var viewModel = (AddFlashcardViewModel) BindingContext;
            viewModel.FrontTextModified();
        }

        private void BackEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!((Entry) sender).IsFocused)
                return; // do not translate when entry was not modified by user
            var viewModel = (AddFlashcardViewModel) BindingContext;
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