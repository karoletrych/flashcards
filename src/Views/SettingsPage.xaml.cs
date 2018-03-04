using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Flashcards.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage ()
		{
			InitializeComponent ();
			Period.Text = Properties.RepetitionPeriod.ToString();
			Period.Completed += UpdatePeriod;
		}

		private void UpdatePeriod(object sender, EventArgs e)
		{
			Properties.RepetitionPeriod = int.Parse(((EntryCell) sender).Text);
		}
	}
}