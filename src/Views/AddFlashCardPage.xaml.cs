using System;
using Flashcards.ViewModels;
using Xamarin.Forms;

namespace Flashcards.Views
{
    public partial class AddFlashcardPage
    {
        public AddFlashcardPage()
        {
            InitializeComponent();
        }

        private void YandexLabel_OnTapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("http://translate.yandex.com"));
        }

	    private async void FrontEntry_OnCompleted(object sender, EventArgs e)
	    {
		    await ((AddFlashcardViewModel) BindingContext).HandleFrontTextChangedByUser();
	    }

	    private async void BackEntry_OnCompleted(object sender, EventArgs e)
	    {
		    await ((AddFlashcardViewModel)BindingContext).HandleBackTextChangedByUser();
		}
    }
}