using FlashCards.ViewModels.Lesson;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlashCards.Views.Lesson
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class AddLessonPage : ContentPage
	{
		public AddLessonPage(AddLessonViewModel addLessonViewModel)
		{
			InitializeComponent();
		    BindingContext = addLessonViewModel;
		}

	    private void FrontEntry_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
	        if(! ((Entry) sender).IsFocused) // do not translate when entry was not modified by user
                return;
	        var viewModel = (AddLessonViewModel) BindingContext;
	            viewModel.FrontTextModified();
	    }

	    private void BackEntry_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
	        if (!((Entry)sender).IsFocused) // do not translate when entry was not modified by user
                return;
            var viewModel = (AddLessonViewModel)BindingContext;
	            viewModel.BackTextModified();
        }
	}
}