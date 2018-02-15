using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using FlashCards.Services;
using FlashCards.Views.Lesson;
using Xamarin.Forms;

namespace FlashCards.Views
{
    public partial class App
    {
        public App(Func<LessonListPage> lessonListPageFactory, Func<ImageImporter> importerFactory)
        {
            var lessonListPage = lessonListPageFactory();

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