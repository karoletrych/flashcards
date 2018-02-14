using Autofac;
using FlashCards.Views.Lesson;
using Xamarin.Forms;

namespace FlashCards.Views
{
    public partial class App
    {
        public App()
        {
            var container = IocRegistrations.RegisterTypesInIocContainer();
            var lessonListPage = container.Resolve<LessonListPage>();

            InitializeComponent();

            MainPage = new NavigationPage(lessonListPage);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}