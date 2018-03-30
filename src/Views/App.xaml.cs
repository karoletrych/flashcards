using System;
using System.Collections.Generic;
using DLToolkit.Forms.Controls;
using Flashcards.Localization;
using Flashcards.PlatformDependentTools;
using Flashcards.Settings;
using Flashcards.SpacedRepetition.Interface;
using Flashcards.ViewModels;
using Prism;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Flashcards.Views
{
    public partial class App
    {
        public App(IPlatformInitializer platformInitializer) : base(platformInitializer)
        {
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LessonListPage, LessonListViewModel>();
            containerRegistry.RegisterForNavigation<AddLessonPage, AddLessonViewModel>();
            containerRegistry.RegisterForNavigation<AddFlashcardPage, AddFlashcardViewModel>();
			containerRegistry.RegisterForNavigation<AskingQuestionsPage, AskingQuestionsViewModel>();
			containerRegistry.RegisterForNavigation<EditLessonPage, EditLessonViewModel>();
			containerRegistry.RegisterForNavigation<SettingsPage>();
        }

		protected override void OnInitialized()
        {
            InitializeComponent();
	        InitializeLocales();
			InitializeSpacedRepetition();
	        FlowListView.Init();

	        NavigationService.NavigateAsync("NavigationPage/LessonListPage");

	        void InitializeLocales()
	        {
		        if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
		        {
			        var localize = Container.Resolve<ILocalize>();
			        var cultureInfo = localize.GetCurrentCultureInfo();
			        AppResources.Culture = cultureInfo;
			        localize.SetLocale(cultureInfo);
		        }
			}

	        void InitializeSpacedRepetition()
	        {
		        var notificationScheduler = Container.Resolve<INotificationScheduler>();
		        var repetitionTimeSetting = Container.Resolve<ISetting<TimeSpan>>();
				notificationScheduler.Schedule(repetitionTimeSetting.Value);

		        var initializers = Container.Resolve<IEnumerable<ISpacedRepetitionInitializer>>();
		        foreach (var spacedRepetitionInitializer in initializers)
		        {
			        spacedRepetitionInitializer.Initialize();
		        }
	        }
        }
	}
}