using System;
using System.Collections.Generic;
using DLToolkit.Forms.Controls;
using Flashcards.Infrastructure.Localization;
using Flashcards.Infrastructure.PlatformDependentTools;
using Flashcards.SpacedRepetition.Interface;
using Prism;
using Prism.Autofac;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Flashcards.Views
{
	public partial class App : PrismApplication
    {
        public App(IPlatformInitializer platformInitializer) : base(platformInitializer)
        {
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
	        ViewModelMappings.RegisterTypes(containerRegistry);
        }

	    protected override void OnInitialized()
        {
            InitializeComponent();
	        InitializeLocales();
			InitializeSpacedRepetition();
	        FlowListView.Init();

	        NavigationService.NavigateAsync("NavigationPage/MainPage");

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
		        var alarmsInitializer = Container.Resolve<IAlarmsInitializer>();
				alarmsInitializer.Initialize();

		        var spacedRepetitionInitializers = Container.Resolve<IEnumerable<ISpacedRepetitionInitializer>>();
		        foreach (var spacedRepetitionInitializer in spacedRepetitionInitializers)
		        {
				    spacedRepetitionInitializer.Initialize();
		        }
	        }
        }
	}
}