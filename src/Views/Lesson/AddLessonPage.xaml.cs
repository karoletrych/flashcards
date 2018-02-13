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
    }
}