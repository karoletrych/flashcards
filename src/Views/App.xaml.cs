using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

	    public override void Initialize()
	    {
		    base.Initialize();

		    InitializeSpacedRepetition();
		    InitializeLocales();
		    FlowListView.Init();

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

		    
		}

	    protected override void OnInitialized()
        {
			InitializeComponent();
	        NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

	    private void InitializeSpacedRepetition()
	    {
		    var alarmsInitializer = Container.Resolve<IAlarmsInitializer>();
		    alarmsInitializer.Initialize();

		    var spacedRepetitionInitializers = Container.Resolve<IEnumerable<ISpacedRepetitionInitializer>>();
		    foreach (var initializer in spacedRepetitionInitializers)
		    {
			    Task.Run(()=>initializer.InitializeAsync()).Wait();
		    }
	    }
	}
}