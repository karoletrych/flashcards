using System;
using Xamarin.Forms;

namespace FlashCards
{
    public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

	    public async void AskQuestions(object sender, EventArgs e)
	    {
	        await Navigation.PushAsync(new QuestionsPage());
        }
    }
}
