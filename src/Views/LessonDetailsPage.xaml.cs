using FlashCards.ViewModels.Lesson;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlashCards.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LessonDetailsPage : ContentPage
	{
		public LessonDetailsPage(LessonViewModel lesson)
		{
			InitializeComponent();
		}
	}
}